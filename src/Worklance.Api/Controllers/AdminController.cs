using Microsoft.AspNetCore.Authorization;
using Worklance.Application.Common.ApiResponse;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Worklance.Application.DTOs.AdminUserDTO;
using Worklance.Application.Interfaces.Queries;
using Worklance.Application.Interfaces.Repositories;

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
            var pendingUsers = await _adminQuery.GetPendingVerificationsAsync();
            return Ok(ApiResponse<IEnumerable<UserVerificationDTO>>.Success(pendingUsers, "Pending verifications retrieved successfully.", 200));
        }

        [HttpPut("{userId}/status")]
        public async Task<IActionResult> UpdateVerificationStatus(
            string userId,
            [FromForm] UpdateVerificationStatusDto request)
        {
            if (request.Status == Domain.Enums.VerificationStatus.Rejected && string.IsNullOrWhiteSpace(request.Reason))
            {
                return BadRequest(ApiResponse<string>.Failure("A reason is required when rejecting a user.", 400));
            }
            var success = await _repository.UpdateStatusAsync(userId, request.Status, request.Reason);

            if (!success)
            {
                return NotFound(ApiResponse<string>.Failure($"Verification record for User {userId} not found.", 404));
            }
            return Ok(ApiResponse<string>.Success($"User {userId} status updated to {request.Status}.", "Success", 200));
        }
    }
}

