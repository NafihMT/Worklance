using System.Text.Json;
using Worklance.Application.DTOs.Category;
using Worklance.Application.DTOs.Jobs;
using Worklance.Application.Exceptions;
using Worklance.Application.Interfaces.CloudinaryInterface;
using Worklance.Application.Interfaces.Repositories;
using Worklance.Application.Interfaces.Services;
using Worklance.Domain.Entities;
using Worklance.Domain.Entities.Job;
using Worklance.Domain.Enums.Job;

namespace Worklance.Application.Services;

public class JobService : IJobService
{
    private readonly IJobRepository _jobRepository;
    private readonly IUnitOfWork _unitOfWork;
    public JobService(IJobRepository jobRepository, IUnitOfWork unitOfWork)
    {
        _jobRepository = jobRepository;
        _unitOfWork = unitOfWork;
    }
    public async Task<JobResponse> CreateJobAsync(string userId, CreateJobRequest request)
    {
        await ValidateJobRequestAsync(request);

        var clientProfileId = await _jobRepository.GetClientProfileIdByUserIdAsync(userId);
        if (clientProfileId == 0)
            throw new NotFoundException("Client profile not found for the current user.");

        byte[]? attachmentBytes = null;
        if (request.AttachmentUrl != null && request.AttachmentUrl.Length > 0)
        {
            using (var ms = new MemoryStream())
            {
                await request.AttachmentUrl.CopyToAsync(ms);
                attachmentBytes = ms.ToArray();
            }
        }

        var job = new Job
        {
            Title = request.Title.Trim(),
            Description = request.Description.Trim(),
            ClientProfileId = clientProfileId,
            CategoryId = request.CategoryId,
            JobType = request.JobType,
            FixedBudget = request.JobType == JobType.Contract ? request.FixedBudget : null,
            MinHourlyRate = request.JobType == JobType.Hourly ? request.MinHourlyRate : null,
            MaxHourlyRate = request.JobType == JobType.Hourly ? request.MaxHourlyRate : null,
            Deadline = request.Deadline,
            Tags = JsonSerializer.Serialize(request.Tags),
            AttachmentBytes = attachmentBytes,
            Status = JobStatus.Open,
            JobSkills = request.SkillIds
                .Select(skillId => new JobSkill { SkillId = skillId })
                .ToList(),
            CreatedAt = DateTime.UtcNow,
            LastModifiedAt = DateTime.UtcNow
        };

        await _jobRepository.AddAsync(job);
        await _unitOfWork.SaveChangesAsync();

        var fullJob = await _jobRepository.GetJobWithDetailsAsync(job.Id)
            ?? throw new NotFoundException("Job creation failed unexpectedly.");

        return MapToResponse(fullJob);
    }

    public async Task<IEnumerable<JobResponse>> GetAllJobsAsync()
    {
        var jobs = await _jobRepository.GetAllJobsWithDetailsAsync();
        return jobs.Select(MapToResponse);
    }

    public async Task<JobResponse> GetJobByIdAsync(int id)
    {
        var job = await _jobRepository.GetJobWithDetailsAsync(id);

        if (job == null)
            throw new NotFoundException($"Job with ID {id} not found.");

        return MapToResponse(job);
    }

    private static JobResponse MapToResponse(Job job)
    {
        return new JobResponse
        {
            JobId = job.Id,
            Title = job.Title,
            Description = job.Description,
            ClientProfileId = job.ClientProfileId,
            ClientName = job.ClientProfile != null ? $"{job.ClientProfile.FirstName} {job.ClientProfile.LastName}" : string.Empty,
            CategoryId = job.CategoryId,
            CategoryName = job.Category?.Name ?? string.Empty,
            JobType = job.JobType,
            Status = job.Status,
            FixedBudget = job.FixedBudget,
            MinHourlyRate = job.MinHourlyRate,
            MaxHourlyRate = job.MaxHourlyRate,
            Deadline = job.Deadline,
            Tags = string.IsNullOrWhiteSpace(job.Tags)
                ? new List<string>()
                : JsonSerializer.Deserialize<List<string>>(job.Tags) ?? new List<string>(),
            Skills = job.JobSkills?.Select(js => js.Skill.Name).ToList() ?? new List<string>(),
            AttachmentUrl = job.AttachmentBytes != null ? Convert.ToBase64String(job.AttachmentBytes) : null,
            CreatedAt = job.CreatedAt
        };
    }

    public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
    {
        var categories = await _jobRepository.GetAllCategoriesAsync();
        return categories.Select(c => new CategoryDto
        {
            Id = c.Id,
            Name = c.Name,
            Description = c.Description,
            Skills = c.Skills.Select(s => new CategorySkillDto
            {
                Id = s.Id,
                Name = s.Name
            }).ToList()
        });
    }

    public async Task<IEnumerable<CategorySkillDto>> GetSkillsByCategoryIdAsync(int categoryId)
    {
        var categoryExists = await _jobRepository.CategoryExistsAsync(categoryId);
        if (!categoryExists)
            throw new NotFoundException($"Category with ID {categoryId} not found.");

        var skills = await _jobRepository.GetSkillsByCategoryIdAsync(categoryId);
        return skills.Select(s => new CategorySkillDto
        {
            Id = s.Id,
            Name = s.Name
        });
    }



    public async Task<JobResponse> UpdateJobAsync(int jobId, string userId, UpdateJobRequest request)
    {
        var job = await GetOwnedJobAsync(jobId, userId);

        if (job == null)
            throw new NotFoundException("Job not found or you don't have permission.");

        if (job.Status != JobStatus.Open)
            throw new BadRequestException("Only open jobs can be edited.");

        await ValidateJobRequestAsync(request);


        byte[]? attachmentBytes = job.AttachmentBytes;
        if (request.AttachmentUrl != null && request.AttachmentUrl.Length > 0)
        {
            using (var ms = new MemoryStream())
            {
                await request.AttachmentUrl.CopyToAsync(ms);
                attachmentBytes = ms.ToArray();
            }
        }

        // Update fields

        job.Title = request.Title.Trim();
        job.Description = request.Description.Trim();
        job.CategoryId = request.CategoryId;
        job.JobType = request.JobType;
        job.FixedBudget = request.JobType == JobType.Contract ? request.FixedBudget : null;
        job.MinHourlyRate = request.JobType == JobType.Hourly ? request.MinHourlyRate : null;
        job.MaxHourlyRate = request.JobType == JobType.Hourly ? request.MaxHourlyRate : null;
        job.Deadline = request.Deadline;
        job.AttachmentBytes = attachmentBytes;
        job.Tags = JsonSerializer.Serialize(request.Tags);

        //Skills

        job.JobSkills.Clear();
        foreach (var skillId in request.SkillIds)
        {
            job.JobSkills.Add(new JobSkill { SkillId = skillId });
        }

        job.LastModifiedAt = DateTime.UtcNow;
        job.LastModifiedBy = userId;

        await _unitOfWork.SaveChangesAsync();

        var updated = await _jobRepository.GetJobWithDetailsAsync(job.Id);
        if (updated is null)
            throw new NotFoundException("Updated job could not be retrieved");

        return MapToResponse(updated);

    }

    public async Task SoftDeleteJobAsync(int jobId, string userId)
    {
        var job = await GetOwnedJobAsync(jobId, userId);
        if (job.IsDeleted)
            throw new BadRequestException("Job is already deleted");

        job.IsDeleted = true;
        job.DeletedAt = DateTime.UtcNow;
        job.LastModifiedAt = DateTime.UtcNow;
        job.LastModifiedBy = userId;

        await _unitOfWork.SaveChangesAsync();

    }

    public async Task CloseJobAsync(int jobId, string userId)
    {
        var job = await GetOwnedJobAsync(jobId, userId);
        if (job.Status != JobStatus.Open)
            throw new BadRequestException("Only open Job can be closed");

        job.Status = JobStatus.Closed;
        job.LastModifiedAt = DateTime.UtcNow;
        job.LastModifiedBy = userId;

        await _unitOfWork.SaveChangesAsync();
    }
    public async Task ReopenJobAsync(int jobId, string userId)
    {
        var job = await GetOwnedJobAsync(jobId, userId);
        if (job.Status != JobStatus.Closed)
            throw new BadRequestException("Only Closed jobs can reopen");

        job.Status = JobStatus.Open;
        job.LastModifiedAt = DateTime.UtcNow;
        job.LastModifiedBy = userId;

        await _unitOfWork.SaveChangesAsync();
    }
    public async Task CancelJobAsync(int jobId, string userId)
    {
        var job = await GetOwnedJobAsync(jobId, userId);
        if (job.Status == JobStatus.Completed)
            throw new BadRequestException("Completed jobs cannot be cancelled");

        if (job.Status == JobStatus.Cancelled)
            throw new BadRequestException("Job is already Cancelled");

        job.Status = JobStatus.Cancelled;
        job.LastModifiedAt = DateTime.UtcNow;
        job.LastModifiedBy = userId;

        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<IEnumerable<JobResponse>> GetMyPostedJobsAsync(string userId, JobStatus? status)
    {
        var clientProfileId = await _jobRepository.GetClientProfileIdByUserIdAsync(userId);
        if (clientProfileId == 0)
            throw new NotFoundException("Client profile not found");

        var jobs = await _jobRepository.GetJobsByClientProfileIdAsync(clientProfileId, status);

        return jobs.Select(MapToResponse);
    }



    // Helper Functions

    private async Task ValidateJobRequestAsync(JobRequestBase request)
    {
        if (!Enum.IsDefined(typeof(JobType), request.JobType))
            throw new BadRequestException("Invalid Job Type");
        var categoryExists = await _jobRepository.CategoryExistsAsync(request.CategoryId);
        if (!categoryExists)
            throw new BadRequestException("The selected category does not exist.");

        if (request.SkillIds.Any())
        {
            var validSkillIds = await _jobRepository.GetExistingSkillIdsAsync(
                request.SkillIds,
                request.CategoryId
                );

            var invalidIds = request.SkillIds.Except(validSkillIds).ToList();

            if (invalidIds.Any())
            {
                throw new BadRequestException(
                    $"Invalid skill ID(s) or they do not belong to the selected category: {string.Join(", ", invalidIds)}");
            }
        }

        if (request.JobType == JobType.Contract)
        {
            if (request.FixedBudget is null || request.FixedBudget <= 0)
                throw new BadRequestException("Fixed budget is required for contract jobs.");

            if (request.MaxHourlyRate is not null || request.MinHourlyRate is not null)
                throw new BadRequestException("Hourly rates are not allowed for contract jobs");
        }
        else if (request.JobType == JobType.Hourly)
        {
            if (request.MinHourlyRate is null || request.MaxHourlyRate is null)
                throw new BadRequestException("Min and max hourly rate are required for hourly jobs.");

            if (request.MinHourlyRate <= 0 || request.MaxHourlyRate <= 0)
                throw new BadRequestException("Hourly rates must be greater than zero.");

            if (request.MinHourlyRate > request.MaxHourlyRate)
                throw new BadRequestException("Min hourly rate cannot exceed max hourly rate.");

            if (request.FixedBudget is not null)
                throw new BadRequestException(
                    "Fixed budget is not allowed for hourly jobs.");
        }

        if (request.Deadline <= DateTime.UtcNow)
            throw new BadRequestException("Deadline must be a future date.");
    }


    private async Task<Job> GetOwnedJobAsync(int jobId, string userId)
    {
        var job = await _jobRepository.GetJobForUpdateAsync(jobId, userId);
        if (job == null)
            throw new ForbiddenException("Job not found or you don't have permission to modify it.");

        return job;
    }
}