using System.Text.Json;
using Worklance.Application.DTOs.Jobs;
using Worklance.Application.Exceptions;
using Worklance.Application.Interfaces.Repositories;
using Worklance.Application.Interfaces.Services;
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
        // Check category exists
        var categoryExists = await _jobRepository.CategoryExistsAsync(request.CategoryId);
        if (!categoryExists)
            throw new ValidationException("The selected category does not exist.");

        if (request.SkillIds.Any())
        {
            var validSkillIds = await _jobRepository.GetExistingSkillIdsAsync(request.SkillIds);
            var invalidIds = request.SkillIds.Except(validSkillIds).ToList();
            if (invalidIds.Any())
                throw new ValidationException(
                    $"Invalid skill ID(s): {string.Join(", ", invalidIds)}");
        }

        if (request.JobType == JobType.Contract)
        {
            if (request.FixedBudget is null or <= 0)
                throw new ValidationException("Fixed budget is required for contract jobs.");
        }

        // Budget Validation
        else if (request.JobType == JobType.Hourly)
        {
            if (request.MinHourlyRate is null || request.MaxHourlyRate is null)
                throw new ValidationException("Min and max hourly rate are required for hourly jobs.");

            if (request.MinHourlyRate <= 0 || request.MaxHourlyRate <= 0)
                throw new ValidationException("Hourly rates must be greater than zero.");

            if (request.MinHourlyRate > request.MaxHourlyRate)
                throw new ValidationException("Min hourly rate cannot exceed max hourly rate.");
        }

        if (request.Deadline <= DateTime.UtcNow)
            throw new ValidationException("Deadline must be a future date.");

        //Resolve ClientProfileId from the JWT userId
        var clientProfileId = await _jobRepository.GetClientProfileIdByUserIdAsync(userId);
        if (clientProfileId == 0)
            throw new NotFoundException("Client profile not found for the current user.");

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
            TagsJson = JsonSerializer.Serialize(request.Tags),
            AttachmentUrl = request.AttachmentUrl,
            Status = JobStatus.Open,
            JobSkills = request.SkillIds
                .Select(skillId => new JobSkill { SkillId = skillId })
                .ToList()
        };

        await _jobRepository.AddAsync(job);
        await _unitOfWork.SaveChangesAsync();

        var fullJob = await _jobRepository.GetJobWithDetailsAsync(job.JobId)
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
            JobId = job.JobId,
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
            Tags = string.IsNullOrWhiteSpace(job.TagsJson)
                ? new List<string>()
                : JsonSerializer.Deserialize<List<string>>(job.TagsJson) ?? new List<string>(),
            Skills = job.JobSkills?.Select(js => js.Skill.Name).ToList() ?? new List<string>(),
            AttachmentUrl = job.AttachmentUrl,
            CreatedAt = job.CreatedAt
        };
    }
}