using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Worklance.Application.DTOs.AuthDTOs;
using Worklance.Application.Interfaces.AuthInterface;
using Worklance.Application.Interfaces.CloudinaryInterface;

namespace Worklance.Api.Controllers.AuthController
{
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
        public async Task<IActionResult> Register([FromForm] RegisterRequestDto request, IFormFile AadhaarProof)
        {
            if (AadhaarProof == null || AadhaarProof.Length == 0)
            {
                return BadRequest("Aadhaar proof file is required.");
            }

            using (var stream = AadhaarProof.OpenReadStream())
            {
                var uploadedUrl = await _cloudinaryService.UploadImageAsync(stream, AadhaarProof.FileName);
                request.AadhaarProof = uploadedUrl;
            }

            var response = await _authService.RegisterAsync(request);

            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp(VerifyOtpRequestDto request)
        {
            var response = await _authService.VerifyOtpAsync(request);

            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromForm] LoginRequestDto request)
        {
            var response = await _authService.LoginAsync(request);

            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken(RefreshTokenRequestDto request)
        {
            var response = await _authService.RefreshTokenAsync(request);

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
    }
}
