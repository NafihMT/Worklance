using System.Threading.Tasks;
using Worklance.Application.Common.ApiResponse;
using Worklance.Application.DTOs.AuthDTOs;
using Worklance.Application.DTOs.ResendOtpRequestDto;

namespace Worklance.Application.Interfaces.AuthInterface
{
    public interface IAuthService
    {
        Task<ApiResponse> RegisterAsync(RegisterRequestDto request);

        Task<ApiResponse> VerifyOtpAsync(VerifyOtpRequestDto request);

        Task<ApiResponse> LoginAsync(LoginRequestDto request);
        Task<ApiResponse> RefreshTokenAsync(RefreshTokenRequestDto request);
        Task<ApiResponse> ForgotPasswordAsync(ForgotPasswordRequestDto request);
        Task<ApiResponse> ResetPasswordAsync(ResetPasswordRequestDto request);
        Task<ApiResponse> LogoutAsync(string refreshToken);
        Task<ApiResponse> ResendOtpAsync(ResendOtpRequestDto request);

    }
}