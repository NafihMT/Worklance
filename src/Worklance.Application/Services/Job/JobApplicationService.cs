using System.IO;
using AutoMapper;
using Worklance.Application.DTOs.FreelancerProfiles;
using Worklance.Application.DTOs.JobApplications;
using Worklance.Application.Exceptions;
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
    private readonly IFileStorageService _fileStorageService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public JobApplicationService(
        IJobApplicationRepository jobApplicationRepository,
        IJobRepository jobRepository,
        IFreelancerProfileRepository freelancerProfileRepository,
        IFileStorageService fileStorageService,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _jobApplicationRepository = jobApplicationRepository;
        _jobRepository = jobRepository;
        _freelancerProfileRepository = freelancerProfileRepository;
        _fileStorageService = fileStorageService;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<JobApplicationResponse> ApplyForJobAsync(int jobId, string userId, ApplyJobRequest request)
    {
        if (string.IsNullOrWhiteSpace(userId) || !int.TryParse(userId, out _))
        {
            throw new UnauthorizedException("User access token is missing or invalid.");
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

        string? coverLetterFileUrl = null;
        string? coverLetterFileName = null;

        if (request.CoverLetterFile != null && request.CoverLetterFile.Length > 0)
        {
            using (var stream = request.CoverLetterFile.OpenReadStream())
            {
                coverLetterFileName = request.CoverLetterFile.FileName;
                coverLetterFileUrl = await _fileStorageService.SaveFileAsync(stream, coverLetterFileName, "cover-letters");
            }
        }

        var application = new JobApplication
        {
            JobId = jobId,
            FreelancerProfileId = freelancerProfile.Id,
            CoverLetterText = request.CoverLetterText?.Trim(),
            CoverLetterFileUrl = coverLetterFileUrl,
            CoverLetterFileName = coverLetterFileName,
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

        var applications = await _jobApplicationRepository.GetByUserIdAsync(userId);
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

    public async Task<IEnumerable<FreelancerProfileDto>> GetApplicantProfilesForJobAsync(int jobId, string userId)
    {
        var applications = await GetApplicationsForJobAsync(jobId, userId);
        return applications
            .Where(a => a.ApplicantProfile != null)
            .Select(a => a.ApplicantProfile!);
    }

    public async Task<(Stream FileStream, string ContentType, string FileName)> DownloadCoverLetterAsync(int applicationId, string userId)
    {
        var application = await _jobApplicationRepository.GetByIdWithDetailsAsync(applicationId);
        if (application == null)
        {
            throw new NotFoundException($"Job application with ID {applicationId} not found.");
        }

        var isApplicant = application.FreelancerProfile != null && application.FreelancerProfile.UserId == userId;
        var isJobOwner = application.Job != null && application.Job.ClientProfile != null && application.Job.ClientProfile.UserId == userId;

        if (!isApplicant && !isJobOwner)
        {
            throw new ForbiddenException("You do not have permission to view or download this cover letter.");
        }

        if (string.IsNullOrEmpty(application.CoverLetterFileUrl))
        {
            throw new NotFoundException("No cover letter file was attached to this application.");
        }

        var stream = await _fileStorageService.GetFileStreamAsync(application.CoverLetterFileUrl);
        var fileName = application.CoverLetterFileName ?? $"cover_letter_{applicationId}.pdf";
        var contentType = GetContentType(fileName);

        return (stream, contentType, fileName);
    }

    private static string GetContentType(string fileName)
    {
        var ext = Path.GetExtension(fileName).ToLowerInvariant();
        return ext switch
        {
            ".pdf" => "application/pdf",
            ".doc" => "application/msword",
            ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            ".txt" => "text/plain",
            _ => "application/octet-stream"
        };
    }

    private JobApplicationResponse MapToResponse(JobApplication application)
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
            FreelancerTitle = application.FreelancerProfile?.ProfessionalTitle,
            FreelancerPhotoUrl = application.FreelancerProfile?.ProfilePhotoUrl,
            FreelancerEmail = application.FreelancerProfile?.Email,
            ApplicantProfile = application.FreelancerProfile != null
                ? _mapper.Map<FreelancerProfileDto>(application.FreelancerProfile)
                : null,
            CoverLetterText = application.CoverLetterText,
            CoverLetterFileUrl = application.CoverLetterFileUrl,
            CoverLetterFileName = application.CoverLetterFileName,
            ProposedRate = application.ProposedRate,
            EstimatedDays = application.EstimatedDays,
            Status = application.Status,
            AppliedAt = application.CreatedAt
        };
    }
}
