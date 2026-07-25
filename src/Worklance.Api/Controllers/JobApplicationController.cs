using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Worklance.Application.Common.ApiResponse;
using Worklance.Application.DTOs.JobApplications;
using Worklance.Application.Interfaces.Services;

namespace Worklance.Api.Controllers;

[ApiController]
[Route("api")]
public class JobApplicationController : ControllerBase
{
    private readonly IJobApplicationService _jobApplicationService;

    public JobApplicationController(IJobApplicationService jobApplicationService)
    {
        _jobApplicationService = jobApplicationService;
    }

    [HttpPost("jobs/{jobId}/apply")]
    [Authorize]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> ApplyForJob(int jobId, [FromForm] ApplyJobRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized(ApiResponse.Failure(
                "Unauthorized access",
                StatusCodes.Status401Unauthorized));
        }

        var result = await _jobApplicationService.ApplyForJobAsync(jobId, userId, request);

        return Ok(ApiResponse.Success(
            result,
            "Job application submitted successfully.",
            StatusCodes.Status200OK));
    }

    
    [HttpGet("job-applications/my-applications")]
    [Authorize]
    public async Task<IActionResult> GetMyApplications()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized(ApiResponse.Failure(
                "Unauthorized access",
                StatusCodes.Status401Unauthorized));
        }

        var result = await _jobApplicationService.GetMyApplicationsAsync(userId);

        return Ok(ApiResponse.Success(
            result,
            "Job applications retrieved successfully.",
            StatusCodes.Status200OK));
    }

    [HttpGet("jobs/{jobId}/applications")]
    [Authorize]
    public async Task<IActionResult> GetApplicationsForJob(int jobId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized(ApiResponse.Failure(
                "Unauthorized access",
                StatusCodes.Status401Unauthorized));
        }

        var result = await _jobApplicationService.GetApplicationsForJobAsync(jobId, userId);

        return Ok(ApiResponse.Success(
            result,
            "Job applications retrieved successfully.",
            StatusCodes.Status200OK));
    }

    [HttpGet("job-applications/{applicationId}/download-cover-letter")]
    [Authorize]
    public async Task<IActionResult> DownloadCoverLetter(int applicationId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized(ApiResponse.Failure(
                "Unauthorized access",
                StatusCodes.Status401Unauthorized));
        }

        var (fileBytes, contentType, fileName) = await _jobApplicationService.DownloadCoverLetterAsync(applicationId, userId);

        return File(fileBytes, contentType, fileName);
    }
}
