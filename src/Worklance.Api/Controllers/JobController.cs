using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Worklance.Application.Common.ApiResponse;
using Worklance.Application.DTOs.Jobs;
using Worklance.Application.Exceptions;
using Worklance.Application.Interfaces.Services;

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
    public async Task<IActionResult> CreateJob([FromBody] CreateJobRequest request)
    {

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
            return Unauthorized(ApiResponse<object>.FailureResponse(
                "Unauthorized Access",
                StatusCodes.Status401Unauthorized));

        var result = await _jobService.CreateJobAsync(userId, request);
        return CreatedAtAction(
            nameof(GetJobById),
            new { id = result.JobId },
            ApiResponse<JobResponse>.SuccessResponse(
                result,
                "Job Created Successfully",
                StatusCodes.Status201Created));

    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetJobs()
    {
        var jobs = await _jobService.GetAllJobsAsync();
        return Ok(ApiResponse<IEnumerable<JobResponse>>.SuccessResponse(
            jobs,
            "Job fetched Successfully"));

    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> GetJobById(int id)
    {
        try
        {
            var job = await _jobService.GetJobByIdAsync(id);
            return Ok(ApiResponse<JobResponse>.SuccessResponse(
                job,
                "Job fetched Successfully"));

        }
        catch (NotFoundException ex)
        {
            return NotFound(ApiResponse<object>.FailureResponse(
                ex.Message,
                StatusCodes.Status404NotFound));
        }
    }

    [HttpGet("categories")]
    public async Task<IActionResult> GetCategories()
    {
        var categories = await _jobService.GetAllCategoriesAsync();
        return Ok(ApiResponse<IEnumerable<CategoryDto>>.SuccessResponse(
            categories,
            "Category Fetched Successfully"));
    }

    [HttpGet("categories/{categoryId}/skills")]
    public async Task<IActionResult> GetSkillsByCategory(int categoryId)
    {
        try
        {
            var skills = await _jobService.GetSkillsByCategoryIdAsync(categoryId);
            return Ok(ApiResponse<IEnumerable<CategorySkillDto>>.SuccessResponse(
                skills,
                "Skills Fetched Successfully"));
        }
        catch (NotFoundException ex)
        {
            return NotFound(ApiResponse<object>.FailureResponse(
                ex.Message,
                StatusCodes.Status404NotFound));
        }
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> UpdateJobAsync(int id, UpdateJobRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var result = await _jobService.UpdateJobAsync(id, userId, request);

        return Ok(ApiResponse<JobResponse>.SuccessResponse(
            result,
            "Job Updated Successfully"));
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteJob(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        await _jobService.SoftDeleteJobAsync(id, userId);

        return Ok(ApiResponse<object>.SuccessResponse(
            null,
            "Job deleted Successfully"));

    }
    
    [HttpPatch("{id}/close")]
    [Authorize]
    public async Task<IActionResult> CloseJob(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        await _jobService.CloseJobAsync(id, userId);

        return Ok(ApiResponse<object>.SuccessResponse(
            null,
            "Job closed successfully."));
    }

    [HttpPatch("{id}/reopen")]
    [Authorize]
    public async Task<IActionResult> ReopenJob(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        await _jobService.ReopenJobAsync(id, userId);

        return Ok(ApiResponse<object>.SuccessResponse(
            null,
            "Job reopened successfully."));
    }

    [HttpPatch("{id}/cancel")]
    [Authorize]
    public async Task<IActionResult> CancelJob(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        await _jobService.CancelJobAsync(id, userId);

        return Ok(ApiResponse<object>.SuccessResponse(
            null,
            "Job cancelled successfully."));
    }
}