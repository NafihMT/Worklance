using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;
using Worklance.Application.DTOs.FreelancerProfiles;
using Worklance.Application.Interfaces.Queries;
using Worklance.Application.Interfaces.Services;

namespace Worklance.Api.Controllers;

[ApiController]
[Route("api/profile")]
public class ProfileController : ControllerBase
{
    private readonly IFreelancerProfileService _profileService;
    private readonly IFreelancerProfileQueryService _profileQueryService;

    public ProfileController(
        IFreelancerProfileService profileService,
        IFreelancerProfileQueryService profileQueryService)
    {
        _profileService = profileService;
        _profileQueryService = profileQueryService;
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateProfile([FromBody] CreateFreelancerProfileDto dto)
    {
        var userId = GetCurrentUserId();
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        if (!ModelState.IsValid) return BadRequest(ModelState);

        try
        {
            var profile = await _profileService.CreateProfileAsync(userId, dto);
            return CreatedAtAction(nameof(GetProfileById), new { id = profile.Id }, profile);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    [Authorize]
    [HttpPut]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateFreelancerProfileDto dto)
    {
        var userId = GetCurrentUserId();
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        if (!ModelState.IsValid) return BadRequest(ModelState);

        try
        {
            var profile = await _profileService.UpdateProfileAsync(userId, dto);
            return Ok(profile);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { Message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> GetMyProfile()
    {
        var userId = GetCurrentUserId();
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var profile = await _profileQueryService.GetMyProfileAsync(userId);
        if (profile == null) return NotFound("Profile not found.");

        return Ok(profile);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetProfileById(int id)
    {
        var profile = await _profileQueryService.GetProfileByIdAsync(id);
        if (profile == null) return NotFound("Profile not found.");

        return Ok(profile);
    }


    [Authorize]
    [HttpPut("resume")]
    public async Task<IActionResult> UploadResume(IFormFile file)
    {
        var userId = GetCurrentUserId();
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        if (file == null || file.Length == 0)
        {
            return BadRequest("File is empty.");
        }

        try
        {
            using var stream = file.OpenReadStream();
            var relativePath = await _profileService.UploadResumeAsync(userId, stream, file.FileName, file.Length);
            return Ok(new { ResumeUrl = relativePath });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { Message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    [Authorize]
    [HttpDelete("resume")]
    public async Task<IActionResult> DeleteResume()
    {
        var userId = GetCurrentUserId();
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        try
        {
            await _profileService.DeleteResumeAsync(userId);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { Message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    [Authorize]
    [HttpGet("resume")]
    public async Task<IActionResult> DownloadResume()
    {
        var userId = GetCurrentUserId();
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        try
        {
            var (stream, contentType, fileName) = await _profileService.DownloadResumeAsync(userId);
            return File(stream, contentType, fileName);
        }
        catch (FileNotFoundException ex)
        {
            return NotFound(new { Message = ex.Message });
        }
    }


    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string? searchTerm, [FromQuery] string? skill, [FromQuery] decimal? minHourlyRate, [FromQuery] decimal? maxHourlyRate)
    {
        var results = await _profileQueryService.SearchFreelancersAsync(searchTerm, skill, minHourlyRate, maxHourlyRate);
        return Ok(results);
    }

    [HttpGet("skills")]
    public async Task<IActionResult> GetSkills()
    {
        var skills = await _profileQueryService.GetAllSkillsAsync();
        return Ok(skills);
    }

    private string? GetCurrentUserId()
    {
        return User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value;
    }
}
