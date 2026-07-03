using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Worklance.Application.Common.ApiResponse;
using Worklance.Application.DTOs.AuthDTOs;

namespace Worklance.Application.Interfaces.AuthInterface
{
    public interface IAuthService
    {
        Task<ApiResponse<string>> RegisterAsync(RegisterRequestDto request);
        Task<ApiResponse<LoginResponseDto>> VerifyOtpAsync(VerifyOtpRequestDto request);
        Task<ApiResponse<LoginResponseDto>> LoginAsync(LoginRequestDto request);
        Task<ApiResponse<RefreshTokenResponseDto>> RefreshTokenAsync(RefreshTokenRequestDto request);
        Task<ApiResponse<string>> ForgotPasswordAsync(ForgotPasswordRequestDto request);
        Task<ApiResponse<string>> ResetPasswordAsync(ResetPasswordRequestDto request);
        Task<ApiResponse<string>> LogoutAsync(string refreshToken);
    }
}
