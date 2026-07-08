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

        try
        {
            var result = await _jobService.CreateJobAsync(userId, request);
            return CreatedAtAction(
                nameof(CreateJob),
                new { id = result.JobId },
                ApiResponse<JobResponse>.SuccessResponse(
                    result,
                    "Job Created Successfully",
                    StatusCodes.Status201Created));
        }
        catch (ValidationException ex)
        {
            return BadRequest(
                ApiResponse<object>.FailureResponse(
                    ex.Message,
                    StatusCodes.Status400BadRequest));
        }
        catch (NotFoundException ex)
        {
            return NotFound(
                ApiResponse<object>.FailureResponse(
                    ex.Message,
                    StatusCodes.Status404NotFound));
        }
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
}