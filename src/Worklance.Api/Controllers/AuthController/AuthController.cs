using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Worklance.Application.DTOs.AuthDTOs;
using Worklance.Application.Interfaces.AuthInterface;
using Worklance.Application.Interfaces.CloudinaryInterface;

namespace Worklance.Api.Controllers.AuthController
{
    public class RegisterApiRequest
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
        public string AadhaarNumber { get; set; } = string.Empty;
        public Worklance.Domain.Enums.AuthEnums.AccountType AccountType { get; set; }
        public IFormFile AadhaarProof { get; set; } = null!;
    }

    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ICloudinaryService _cloudinaryService;

        public AuthController(IAuthService authService, ICloudinaryService cloudinaryService)
        {
            _authService = authService;
            _cloudinaryService = cloudinaryService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm, Bind("FullName", "Email", "PhoneNumber", "Password", "ConfirmPassword", "AadhaarNumber", "AccountType")] RegisterRequestDto request)
        {
            var logPath = @"C:\Users\HP\source\repos\Worklance\crashlog.txt";
            System.IO.File.AppendAllText(logPath, "1. Register endpoint hit.\n");
            
            var AadhaarProof = Request.Form.Files["AadhaarProof"];
            if (AadhaarProof == null || AadhaarProof.Length == 0)
            {
                System.IO.File.AppendAllText(logPath, "Error: Aadhaar proof file is required.\n");
                return BadRequest("Aadhaar proof file is required.");
            }

            System.IO.File.AppendAllText(logPath, "2. Uploading image to Cloudinary...\n");
            using (var stream = AadhaarProof.OpenReadStream())
            {
                var uploadedUrl = await _cloudinaryService.UploadImageAsync(stream, AadhaarProof.FileName);
                request.AadhaarProof = uploadedUrl;
            }

            System.IO.File.AppendAllText(logPath, "3. Calling AuthService.RegisterAsync...\n");
            var response = await _authService.RegisterAsync(request);

            System.IO.File.AppendAllText(logPath, "4. Registration successful.\n");
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
        public async Task<IActionResult> VerifyOtp(VerifyOtpRequestDto request)
        {
            var response = await _authService.VerifyOtpAsync(request);
            if (response.Success && response.Data != null)
            {
                SetTokensInCookies(response.Data.AccessToken, response.Data.RefreshToken);
            }
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromForm] LoginRequestDto request)
        {
            var response = await _authService.LoginAsync(request);
            if (response.Success && response.Data != null)
            {
                SetTokensInCookies(response.Data.AccessToken, response.Data.RefreshToken);
            }
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                return BadRequest(Worklance.Application.Common.ApiResponse.ApiResponse<string>.FailureResponse("Refresh token is missing from cookies.", 400));
            }

            var request = new RefreshTokenRequestDto { RefreshToken = refreshToken };
            var response = await _authService.RefreshTokenAsync(request);

            if (response.Success && response.Data != null)
            {
                SetTokensInCookies(response.Data.AccessToken, response.Data.RefreshToken);
            }

            return StatusCode(response.StatusCode, response);
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
            return Ok(new { Message = "Authenticated successfully!", Claims = claims });
        }
    }
}
