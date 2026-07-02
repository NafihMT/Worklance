using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Worklance.Application.DTOs.Jobs;
using Worklance.Application.Exceptions;
using Worklance.Application.Interfaces;
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
    //[Authorize(Roles = "Client,Worker")]
    public async Task<IActionResult> CreateJob([FromBody] CreateJobRequest request)
    {

        //var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var userId = "test-user-id";
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        try
        {
            var result = await _jobService.CreateJobAsync(userId, request);
            return CreatedAtAction(nameof(CreateJob), new { id = result.JobId }, result);
        }
        catch (ValidationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpGet]
    //[Authorize] 
    public async Task<IActionResult> GetJobs()
    {
        var jobs = await _jobService.GetAllJobsAsync();
        return Ok(jobs);
    }

    [HttpGet("{id}")]
    //[Authorize]
    public async Task<IActionResult> GetJobById(int id)
    {
        try
        {
            var job = await _jobService.GetJobByIdAsync(id);
            return Ok(job);
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}