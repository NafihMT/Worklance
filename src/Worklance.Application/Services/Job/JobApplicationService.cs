using System.IO;
using AutoMapper;
using Worklance.Application.DTOs.JobApplications;
using Worklance.Application.Exceptions;
using Worklance.Application.Interfaces.AuthInterface;
using Worklance.Application.Interfaces.Repositories;
using Worklance.Application.Interfaces.Services;
using Worklance.Domain.Entities.Job;
using Worklance.Domain.Enums.Job;

namespace Worklance.Application.Services.Jobs;

public class JobApplicationService : IJobApplicationService
{
    private readonly IJobApplicationRepository _jobApplicationRepository;
    private readonly IJobRepository _jobRepository;
    private readonly IFreelancerProfileRepository _freelancerProfileRepository;
    private readonly IAuthRepository _authRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public JobApplicationService(
        IJobApplicationRepository jobApplicationRepository,
        IJobRepository jobRepository,
        IFreelancerProfileRepository freelancerProfileRepository,
        IAuthRepository authRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _jobApplicationRepository = jobApplicationRepository;
        _jobRepository = jobRepository;
        _freelancerProfileRepository = freelancerProfileRepository;
        _authRepository = authRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<JobApplicationResponse> ApplyForJobAsync(int jobId, string userId, ApplyJobRequest request)
    {
        if (string.IsNullOrWhiteSpace(userId) || !int.TryParse(userId, out var userIntId))
        {
            throw new UnauthorizedException("User access token is missing or invalid.");
        }

        var user = await _authRepository.GetByIdAsync(userIntId);
        if (user == null)
        {
            throw new UnauthorizedException("User account not found.");
        }

        var freelancerProfile = await _freelancerProfileRepository.GetProfileWithDetailsAsync(userId);
        if (freelancerProfile == null)
        {
            throw new BadRequestException("You must complete your freelancer profile before applying for a job.");
        }

        var job = await _jobRepository.GetJobWithDetailsAsync(jobId);
        if (job == null || job.IsDeleted)
        {
            throw new NotFoundException($"Job with ID {jobId} not found.");
        }

        if (!job.IsOpen || job.Status != JobStatus.Open)
        {
            throw new BadRequestException("This job is not currently open for applications. Applications can only be submitted for open jobs.");
        }

        if (job.Deadline <= DateTime.UtcNow)
        {
            throw new BadRequestException("The deadline to apply for this job has passed.");
        }

        // Client cannot apply to their own job
        if (job.ClientProfile != null && job.ClientProfile.UserId == userId)
        {
            throw new BadRequestException("You cannot apply to your own job posting.");
        }

        var alreadyApplied = await _jobApplicationRepository.HasAlreadyAppliedAsync(jobId, freelancerProfile.Id);
        if (alreadyApplied)
        {
            throw new BadRequestException("You have already applied for this job.");
        }

        // Validate ProposedRate is above/equal to the base bid / budget
        if (job.JobType == JobType.Contract && job.FixedBudget.HasValue)
        {
            if (request.ProposedRate < job.FixedBudget.Value)
            {
                throw new BadRequestException($"Proposed rate must be at least the fixed budget of {job.FixedBudget.Value}.");
            }
        }
        else if (job.JobType == JobType.Hourly && job.MinHourlyRate.HasValue)
        {
            if (request.ProposedRate < job.MinHourlyRate.Value)
            {
                throw new BadRequestException($"Proposed rate must be at least the minimum hourly rate of {job.MinHourlyRate.Value}.");
            }
        }

        if (request.CoverLetterFile == null || request.CoverLetterFile.Length == 0)
        {
            throw new BadRequestException("Cover letter file is required.");
        }

        byte[] coverLetterBytes;
        using (var ms = new MemoryStream())
        {
            await request.CoverLetterFile.CopyToAsync(ms);
            coverLetterBytes = ms.ToArray();
        }

        var application = new JobApplication
        {
            JobId = jobId,
            FreelancerProfileId = freelancerProfile.Id,
            CoverLetterBytes = coverLetterBytes,
            CoverLetterFileName = request.CoverLetterFile.FileName,
            CoverLetterContentType = request.CoverLetterFile.ContentType ?? "application/octet-stream",
            ProposedRate = request.ProposedRate,
            EstimatedDays = request.EstimatedDays,
            Status = JobApplicationStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = userId,
            LastModifiedAt = DateTime.UtcNow,
            LastModifiedBy = userId
        };

        await _jobApplicationRepository.AddAsync(application);
        await _unitOfWork.SaveChangesAsync();

        var createdApplication = await _jobApplicationRepository.GetByIdWithDetailsAsync(application.Id);
        return MapToResponse(createdApplication ?? application);
    }

    public async Task<IEnumerable<JobApplicationResponse>> GetMyApplicationsAsync(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            throw new UnauthorizedException("User context missing.");
        }

        var freelancerProfile = await _freelancerProfileRepository.GetProfileWithDetailsAsync(userId);
        if (freelancerProfile == null)
        {
            throw new BadRequestException("Freelancer profile not found.");
        }

        var applications = await _jobApplicationRepository.GetApplicationsByFreelancerIdAsync(freelancerProfile.Id);
        return applications.Select(MapToResponse);
    }

    public async Task<IEnumerable<JobApplicationResponse>> GetApplicationsForJobAsync(int jobId, string userId)
    {
        var job = await _jobRepository.GetJobWithDetailsAsync(jobId);
        if (job == null || job.IsDeleted)
        {
            throw new NotFoundException($"Job with ID {jobId} not found.");
        }

        if (job.ClientProfile == null || job.ClientProfile.UserId != userId)
        {
            throw new ForbiddenException("You do not have permission to view applications for this job.");
        }

        var applications = await _jobApplicationRepository.GetApplicationsByJobIdAsync(jobId);
        return applications.Select(MapToResponse);
    }

    public async Task<(byte[] FileBytes, string ContentType, string FileName)> DownloadCoverLetterAsync(int applicationId, string userId)
    {
        var application = await _jobApplicationRepository.GetByIdWithDetailsAsync(applicationId);
        if (application == null)
        {
            throw new NotFoundException($"Job application with ID {applicationId} not found.");
        }

        // Authorize: Either the freelancer applicant or the job owner (client) can access the cover letter file
        var isApplicant = application.FreelancerProfile != null && application.FreelancerProfile.UserId == userId;
        var isJobOwner = application.Job != null && application.Job.ClientProfile != null && application.Job.ClientProfile.UserId == userId;

        if (!isApplicant && !isJobOwner)
        {
            throw new ForbiddenException("You do not have permission to view or download this cover letter.");
        }

        return (application.CoverLetterBytes, application.CoverLetterContentType, application.CoverLetterFileName);
    }

    private static JobApplicationResponse MapToResponse(JobApplication application)
    {
        return new JobApplicationResponse
        {
            Id = application.Id,
            JobId = application.JobId,
            JobTitle = application.Job?.Title ?? string.Empty,
            FreelancerProfileId = application.FreelancerProfileId,
            FreelancerName = application.FreelancerProfile != null
                ? $"{application.FreelancerProfile.FirstName} {application.FreelancerProfile.LastName}".Trim()
                : string.Empty,
            CoverLetterFileName = application.CoverLetterFileName,
            CoverLetterContentType = application.CoverLetterContentType,
            ProposedRate = application.ProposedRate,
            EstimatedDays = application.EstimatedDays,
            Status = application.Status,
            AppliedAt = application.CreatedAt
        };
    }
}
