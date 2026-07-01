using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Worklance.Application.DTOs.Requests;
using Worklance.Application.Interfaces.Queries;
using Worklance.Application.Interfaces.Repositories;

namespace Worklance.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
            return Ok(pendingUsers);
        }

        [HttpPost("approve/{userId}")]
        public async Task<IActionResult> ApproveUser(string userId)
        {
            return Ok(new { message = $"User {userId} approved successfully." });
        }

        [HttpPost("reject/{userId}")]
        public async Task<IActionResult> RejectUser(string userId)
        {

            return Ok(new { message = $"User {userId} was rejected." });
        }

        [HttpPut("{userId}/status")]
        public async Task<IActionResult> UpdateVerificationStatus(
            string userId,
            [FromBody] UpdateVerificationStatusDto request)
        {
            if (request.Status == Domain.Enums.VerificationStatus.Rejected && string.IsNullOrWhiteSpace(request.Reason))
            {
                return BadRequest("A reason is required when rejecting a user.");
            }
            var success = await _repository.UpdateStatusAsync(userId, request.Status, request.Reason);

            if (!success)
            {
                return NotFound($"Verification record for User {userId} not found.");
            }
            return Ok(new { message = $"User {userId} status updated to {request.Status}." });
        }
    }
}

