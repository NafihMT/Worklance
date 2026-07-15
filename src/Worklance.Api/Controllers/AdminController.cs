using Microsoft.AspNetCore.Authorization;
using Worklance.Application.Common.ApiResponse;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Worklance.Application.DTOs.AdminUserDTO;
using Worklance.Application.Interfaces.Queries;
using Worklance.Application.Interfaces.Repositories;
using System.Security.Claims;

namespace Worklance.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "2")] 
    public class AdminController : ControllerBase
    {
        private readonly IAdminUserVerificationQuery _adminQuery;
        private readonly IUserVerificationRepository _repository;

        public AdminController(IAdminUserVerificationQuery adminQuery, IUserVerificationRepository repository)
        {
            _adminQuery = adminQuery;
            _repository = repository;
        }

        [HttpGet("pending-verification")]
        public async Task<IActionResult> GetPendingVerifications()
        {
            try
            {
                var pendingUsers = await _adminQuery.GetPendingVerificationsAsync();

                if (pendingUsers == null || !pendingUsers.Any())
                {
                    return Ok(ApiResponse.Success(pendingUsers, "No pending verifications found at this time.", 200));
                }

                return Ok(ApiResponse.Success(pendingUsers, "Pending verifications retrieved successfully.", 200));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse.Failure("An error occurred while fetching pending users. Please try again later.", 500));
            }
        }

        [HttpGet("{userId}/aadhaar-image")]
        public async Task<IActionResult> GetUserAadhaarImage(string userId)
        {
            var dbResult = await _adminQuery.GetUserAadhaarImageAsync(userId);

            if (dbResult == null)
            {
                return NotFound(ApiResponse.Failure("Aadhaar image not found for this user.", 404));
            }

            var imageData = new { UserName = dbResult.Value.FullName };

            string finalBase64 = Convert.ToBase64String(dbResult.Value.ImageBytes);

            return Ok(ApiResponse.Success(
                imageData,
                finalBase64,        
                $"Image retrieved for {dbResult.Value.FullName}.",
                200
            ));
        }

        [HttpPut("{userId}/status")]
        public async Task<IActionResult> UpdateVerificationStatus(
            string userId,
            [FromForm] UpdateVerificationStatusDto request)
        {
            var currentLoggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == currentLoggedInUserId)
            {
                return BadRequest(ApiResponse.Failure("Action Denied: You cannot update your own verification status.", 400));
            }

            if (request.Status == Domain.Enums.VerificationStatus.Rejected && string.IsNullOrWhiteSpace(request.Reason))
            {
                return BadRequest(ApiResponse.Failure("A reason is required when rejecting a user.", 400));
            }

            var result = await _repository.UpdateStatusAsync(userId, request.Status, request.Reason);

            if (!result.IsSuccess)
            {
                return BadRequest(ApiResponse.Failure(result.ErrorMessage!, 400));
            }

            return Ok(ApiResponse.Success($"User {userId} status updated to {request.Status}.", 200));
        }
    }
}