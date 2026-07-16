using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Worklance.Application.Common.ApiResponse;
using Worklance.Application.DTOs.Jobs;
using Worklance.Application.Interfaces.Services;
using Worklance.Domain.Enums.Job;

namespace Worklance.API.Controllers;

[ApiController]
[Route("api/jobs")]
public class JobsController : ControllerBase
{
    private readonly IJobService _jobService;

    public JobsController(IJobService jobService)
    {
        _jobService = jobService;
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateJob([FromForm] CreateJobRequest request)
    {

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
            return Unauthorized(ApiResponse<object>.Failure(
                "Unauthorized access",
                StatusCodes.Status401Unauthorized));

        var result = await _jobService.CreateJobAsync(userId, request);
        return CreatedAtAction(
            nameof(GetJobById),
            new { id = result.JobId },
            ApiResponse<JobResponse>.Success(
                result,
                "Job created successfully",
                StatusCodes.Status201Created));

    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetJobs()
    {
        var jobs = await _jobService.GetAllJobsAsync();
        return Ok(ApiResponse<IEnumerable<JobResponse>>.Success(
            jobs,
            "Job retrieved successfully."));

    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> GetJobById(int id)
    {
        var job = await _jobService.GetJobByIdAsync(id);
        return Ok(ApiResponse<JobResponse>.Success(
            job,
            "Job retrieved successfully"));
    }

    [HttpGet("categories")]
    public async Task<IActionResult> GetCategories()
    {
        var categories = await _jobService.GetAllCategoriesAsync();
        return Ok(ApiResponse<IEnumerable<CategoryDto>>.Success(
            categories,
            "Category retrieved successfully"));
    }

    [HttpGet("categories/{categoryId}/skills")]
    public async Task<IActionResult> GetSkillsByCategory(int categoryId)
    {
        var skills = await _jobService.GetSkillsByCategoryIdAsync(categoryId);
        return Ok(ApiResponse<IEnumerable<CategorySkillDto>>.Success(
            skills,
            "Skills retrieved successfully"));
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> UpdateJob(int id, [FromForm] UpdateJobRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized(
             ApiResponse<object>.Failure(
                 "Unauthorized access",
                 StatusCodes.Status401Unauthorized));

        var result = await _jobService.UpdateJobAsync(id, userId, request);

        return Ok(ApiResponse<JobResponse>.Success(
            result,
            "Job updated successfully"));
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteJob(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized(
             ApiResponse<object>.Failure(
                 "Unauthorized access",
                 StatusCodes.Status401Unauthorized));

        await _jobService.SoftDeleteJobAsync(id, userId);

        return Ok(ApiResponse<object>.Success(

            "Job deleted successfully"));

    }

    [HttpPatch("{id}/close")]
    [Authorize]
    public async Task<IActionResult> CloseJob(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized(
                ApiResponse<object>.Failure(
                    "Unauthorized access",
                    StatusCodes.Status401Unauthorized));
        }

        await _jobService.CloseJobAsync(id, userId);

        return Ok(ApiResponse<object>.Success(

            "Job closed successfully."));
    }

    [HttpPatch("{id}/reopen")]
    [Authorize]
    public async Task<IActionResult> ReopenJob(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized(
                ApiResponse<object>.Failure(
                    "Unauthorized access",
                    StatusCodes.Status401Unauthorized));
        }

        await _jobService.ReopenJobAsync(id, userId);

        return Ok(ApiResponse<object>.Success(

            "Job reopened successfully."));
    }

    [HttpPatch("{id}/cancel")]
    [Authorize]
    public async Task<IActionResult> CancelJob(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized(
                ApiResponse<object>.Failure(
                    "Unauthorized access",
                    StatusCodes.Status401Unauthorized));
        }

        await _jobService.CancelJobAsync(id, userId);

        return Ok(ApiResponse<object>.Success(

            "Job cancelled successfully."));
    }

    [HttpGet("my-jobs")]
    [Authorize]
    public async Task<IActionResult> GetMyPostedJobs([FromQuery] JobStatus? status)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized(
                ApiResponse<object>.Failure(
                    "Unauthorized access",
                    StatusCodes.Status401Unauthorized));

        var jobs = await _jobService.GetMyPostedJobsAsync(userId, status);

        return Ok(ApiResponse<IEnumerable<JobResponse>>.Success(
            jobs,
            "Jobs retrieved successfully"));

    }
}