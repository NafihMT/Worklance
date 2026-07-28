using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Worklance.Application.Common.ApiResponse;
using Worklance.Application.DTOs.AuthDTOs;
using Worklance.Application.DTOs.ResendOtpRequestDto;
using Worklance.Application.Interfaces.AuthInterface;

namespace Worklance.Api.Controllers.AuthController
{
    public class RegisterApiRequest
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public long PhoneNumber { get; set; }
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
        public long AadhaarNumber { get; set; }
        public Worklance.Domain.Enums.AuthEnums.AccountType AccountType { get; set; }
        public IFormFile AadhaarProof { get; set; } = null!;
    }

    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm] RegisterApiRequest formRequest)
        {
            var requestDto = new RegisterRequestDto
            {
                FullName = formRequest.FullName,
                Email = formRequest.Email,
                PhoneNumber = formRequest.PhoneNumber,
                Password = formRequest.Password,
                ConfirmPassword = formRequest.ConfirmPassword,
                AadhaarNumber = formRequest.AadhaarNumber,
                AccountType = formRequest.AccountType
            };

            if (formRequest.AadhaarProof != null && formRequest.AadhaarProof.Length > 0)
            {
                using (var ms = new MemoryStream())
                {
                    await formRequest.AadhaarProof.CopyToAsync(ms);
                    requestDto.AadhaarImageBytes = ms.ToArray();
                }
            }
            else
            {
                requestDto.AadhaarImageBytes = Array.Empty<byte>();
            }

            var response = await _authService.RegisterAsync(requestDto);
            return StatusCode(response.StatusCode, response);
        }
        private void SetTokensInCookies(string accessToken, string refreshToken)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = System.DateTime.UtcNow.AddDays(7),
                Secure = true,
                SameSite = SameSiteMode.None
            };
            if (!string.IsNullOrEmpty(accessToken))
            {
                Response.Cookies.Append("accessToken", accessToken, cookieOptions);
            }
            if (!string.IsNullOrEmpty(refreshToken))
            {
                Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
            }
        }

        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpRequestDto request)
        {
            if (request == null) return BadRequest(ApiResponse.Failure("Request cannot be empty.", 400));

            var response = await _authService.VerifyOtpAsync(request);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("resend-otp")]
        public async Task<IActionResult> ResendOtp([FromBody] ResendOtpRequestDto request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Email))
            {
                return BadRequest(ApiResponse.Failure("Email is required.", 400));
            }

            var response = await _authService.ResendOtpAsync(request);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromForm] LoginRequestDto request)
        {
            var response = await _authService.LoginAsync(request);

            if (response is SuccessResponse successResponse && successResponse.Data != null)
            {
                dynamic tokenData = successResponse.Data;
                SetTokensInCookies(tokenData.AccessToken, tokenData.RefreshToken);
            }
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                return BadRequest(ApiResponse.Failure("Refresh token is missing from cookies.", 400));
            }

            var request = new RefreshTokenRequestDto { RefreshToken = refreshToken };
            var response = await _authService.RefreshTokenAsync(request);

            if (response is SuccessResponse successResponse && successResponse.Data != null)
            {
                dynamic tokenData = successResponse.Data;
                SetTokensInCookies(tokenData.AccessToken, tokenData.RefreshToken);
            }

            return StatusCode(response.StatusCode, response);
        }

        private void ClearCookies()
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = System.DateTime.UtcNow.AddDays(-1),
                Secure = true,
                SameSite = SameSiteMode.None
            };

            Response.Cookies.Append("accessToken", "", cookieOptions);
            Response.Cookies.Append("refreshToken", "", cookieOptions);
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var refreshToken = Request.Cookies["refreshToken"];

            if (!string.IsNullOrWhiteSpace(refreshToken))
            {
                await _authService.LogoutAsync(refreshToken);
            }

            ClearCookies();
            return Ok(ApiResponse.Success("Logged out successfully.", 200));
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordRequestDto request)
        {
            var response = await _authService.ForgotPasswordAsync(request);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequestDto request)
        {
            var response = await _authService.ResetPasswordAsync(request);
            return StatusCode(response.StatusCode, response);
        }

        [Authorize]
        [HttpGet("test-auth")]
        public IActionResult TestAuth()
        {
            var claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();
            return Ok(ApiResponse.Success(claims, "Authenticated successfully!", 200));
        }
    }
}