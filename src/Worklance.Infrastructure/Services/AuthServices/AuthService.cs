using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Worklance.Application.DTOs.AuthDTOs;
using Worklance.Application.Interfaces.AuthInterface;
using Worklance.Application.Interfaces.CloudinaryInterface;
using Worklance.Application.Interfaces.EmailInterface;
using Worklance.Domain.Entities.AuthEntities;
using Worklance.Infrastructure.Data.Repositories.AuthRepo;
using Worklance.Infrastructure.Services.Cloudinary;
using Worklance.Infrastructure.Services.Email;
using Worklance.Application.Common.ApiResponse;
using Worklance.Application.Common.OTP;
using Worklance.Domain.Enums;
using Worklance.Domain.Enums.AuthEnums;
using Worklance.Application.Exceptions;
using FluentValidation;
using Microsoft.Extensions.Caching.Memory;

namespace Worklance.Infrastructure.Services.AuthServices
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly IEmailService _emailService;
        private readonly IOcrService _ocrService;
        private readonly IJwtService _jwtService;
        private readonly IValidator<RegisterRequestDto> _registerValidator;
        private readonly IValidator<LoginRequestDto> _loginValidator;
        private readonly IMemoryCache _memoryCache;

        public AuthService(
            IAuthRepository authRepository,
            ICloudinaryService cloudinaryService,
            IEmailService emailService,
            IOcrService ocrService,
            IJwtService jwtService,
            IValidator<RegisterRequestDto> registerValidator,
            IValidator<LoginRequestDto> loginValidator,
            IMemoryCache memoryCache)
        {
            _authRepository = authRepository;
            _cloudinaryService = cloudinaryService;
            _emailService = emailService;
            _ocrService = ocrService;
            _jwtService = jwtService;
            _registerValidator = registerValidator;
            _loginValidator = loginValidator;
            _memoryCache = memoryCache;
        }

        public async Task<ApiResponse<string>> RegisterAsync(RegisterRequestDto request)
        {
            var validationResult = await _registerValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                throw new BadRequestException(validationResult.Errors.First().ErrorMessage);
            }

            await ValidateDuplicateAsync(request);

            var passwordHash = HashPassword(request.Password);
            var otp = OtpGenerator.GenerateOtp();

            var tempUser = new TempUserRegistration
            {
                Request = request,
                PasswordHash = passwordHash,
                OtpCode = otp,
                ExpiresAt = DateTime.UtcNow.AddMinutes(5)
            };

            _memoryCache.Set(request.Email, tempUser, TimeSpan.FromMinutes(5));

            await _emailService.SendOtpAsync(request.Email, otp);

            return ApiResponse<string>.SuccessResponse(
                "Registration successful. OTP has been sent to your email.",
                "Success",
                201);
        }

        private async Task ValidateDuplicateAsync(RegisterRequestDto request)
        {
            if (await _authRepository.EmailExistsAsync(request.Email))
            {
                throw new BadRequestException("Email already exists.");
            }

            if (await _authRepository.PhoneNumberExistsAsync(request.PhoneNumber))
            {
                throw new BadRequestException("Phone number already exists.");
            }

            if (await _authRepository.AadhaarNumberExistsAsync(request.AadhaarNumber))
            {
                throw new BadRequestException("Aadhaar number already exists.");
            }
        }


        private async Task VerifyAadhaarAsync(string imageUrl, string aadhaarNumber)
        {
            var extractedAadhaar = await _ocrService.ReadAadhaarNumberAsync(imageUrl);
            if (string.IsNullOrEmpty(extractedAadhaar))
            {
                throw new BadRequestException("Failed to extract Aadhaar number from the provided Aadhaar proof image.");
            }

            if (extractedAadhaar != aadhaarNumber)
            {
                throw new BadRequestException("The provided Aadhaar number does not match the number extracted from the Aadhaar proof.");
            }
        }

        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        private User CreateUser(RegisterRequestDto request, string passwordHash, string imageUrl)
        {
            return new User
            {
                FullName = request.FullName,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                PasswordHash = passwordHash,
                AadhaarNumber = request.AadhaarNumber,
                AadhaarProofPath = imageUrl,
                AccountType = request.AccountType,
                EmailVerified = false,
                Status = UserStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };
        }
        private async Task SaveUserAsync(User user)
        {
            await _authRepository.AddUserAsync(user);

            await _authRepository.SaveChangesAsync();
        }
        private string GenerateOtp()
        {
            throw new NotImplementedException();
        }

        private async Task SaveOtpAsync(User user, string otp)
        {
            var emailOtp = new EmailOtp
            {
                UserId = user.Id,
                OtpCode = otp,
                ExpiresAt = DateTime.UtcNow.AddMinutes(5),
                IsUsed = false
            };

            await _authRepository.AddEmailOtpAsync(emailOtp);

            await _authRepository.SaveChangesAsync();
        }
        public async Task<ApiResponse<LoginResponseDto>> VerifyOtpAsync(VerifyOtpRequestDto request)
        {
            if (!_memoryCache.TryGetValue(request.Email, out TempUserRegistration? tempUser) || tempUser == null)
            {
                throw new BadRequestException("Invalid or expired OTP.");
            }

            if (tempUser.OtpCode != request.Otp)
            {
                throw new BadRequestException("Invalid OTP.");
            }

            if (tempUser.ExpiresAt < DateTime.UtcNow)
            {
                _memoryCache.Remove(request.Email);
                throw new BadRequestException("OTP has expired.");
            }

            var user = CreateUser(tempUser.Request, tempUser.PasswordHash, tempUser.Request.AadhaarProof);
            user.EmailVerified = true;
            // Admin verification remains Pending until Admin approves
            user.Status = UserStatus.Pending;

            await _authRepository.AddUserAsync(user);
            await _authRepository.SaveChangesAsync();

            _memoryCache.Remove(request.Email);

            var accessToken = _jwtService.GenerateAccessToken(user);
            var refreshTokenEntity = _jwtService.GenerateRefreshToken();
            refreshTokenEntity.UserId = user.Id;

            await _authRepository.AddRefreshTokenAsync(refreshTokenEntity);
            await _authRepository.SaveChangesAsync();

            var response = new LoginResponseDto
            {
                UserId = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                AccessToken = accessToken,
                RefreshToken = refreshTokenEntity.Token
            };

            return ApiResponse<LoginResponseDto>.SuccessResponse(response, "Email verified successfully.", 200);
        }
        private EmailOtp CreateEmailOtp(User user,string otp)
        {
            return new EmailOtp
            {
                User = user,
                OtpCode = otp,
                ExpiresAt = DateTime.UtcNow.AddMinutes(5),
                IsUsed = false
            };
        }
        public async Task<ApiResponse<LoginResponseDto>> LoginAsync(LoginRequestDto request)
        {
            var validationResult = await _loginValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                throw new BadRequestException(validationResult.Errors.First().ErrorMessage);
            }

            var user = await _authRepository.GetUserByEmailAsync(request.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                throw new BadRequestException("Invalid email or password.");
            }

            if (!user.EmailVerified)
            {
                throw new BadRequestException("Please verify your email first.");
            }

            var accessToken = _jwtService.GenerateAccessToken(user);
            var refreshTokenEntity = _jwtService.GenerateRefreshToken();
            refreshTokenEntity.UserId = user.Id;

            await _authRepository.AddRefreshTokenAsync(refreshTokenEntity);
            await _authRepository.SaveChangesAsync();

            var response = new LoginResponseDto
            {
                UserId = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                AccessToken = accessToken,
                RefreshToken = refreshTokenEntity.Token
            };

            return ApiResponse<LoginResponseDto>.SuccessResponse(response, "Login successful.", 200);
        }
        public async Task<ApiResponse<RefreshTokenResponseDto>> RefreshTokenAsync(RefreshTokenRequestDto request)
        {
            var storedToken = await _authRepository.GetRefreshTokenAsync(request.RefreshToken);
            if (storedToken == null || storedToken.IsRevoked || storedToken.ExpiresAt < DateTime.UtcNow)
            {
                throw new BadRequestException("Invalid or expired refresh token.");
            }

            var user = storedToken.User;

            storedToken.IsRevoked = true;
            await _authRepository.UpdateRefreshTokenAsync(storedToken);

            var newAccessToken = _jwtService.GenerateAccessToken(user);
            var newRefreshTokenEntity = _jwtService.GenerateRefreshToken();
            newRefreshTokenEntity.UserId = user.Id;

            await _authRepository.AddRefreshTokenAsync(newRefreshTokenEntity);
            await _authRepository.SaveChangesAsync();

            var response = new RefreshTokenResponseDto
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshTokenEntity.Token
            };

            return ApiResponse<RefreshTokenResponseDto>.SuccessResponse(response, "Token refreshed successfully.", 200);
        }
        public async Task<ApiResponse<string>> ForgotPasswordAsync(ForgotPasswordRequestDto request)
        {
            var user = await _authRepository.GetUserByEmailAsync(request.Email);
            if (user == null)
            {
                throw new BadRequestException("User with this email does not exist.");
            }

            var otp = OtpGenerator.GenerateOtp();

            var emailOtp = new EmailOtp
            {
                UserId = user.Id,
                OtpCode = otp,
                ExpiresAt = DateTime.UtcNow.AddMinutes(5),
                IsUsed = false
            };

            await _authRepository.AddEmailOtpAsync(emailOtp);
            await _authRepository.SaveChangesAsync();

            await _emailService.SendOtpAsync(user.Email, otp);

            return ApiResponse<string>.SuccessResponse("Password reset OTP has been sent to your email.", "Success", 200);
        }
        public async Task<ApiResponse<string>> ResetPasswordAsync(ResetPasswordRequestDto request)
        {
            if (request.NewPassword != request.ConfirmPassword)
            {
                throw new BadRequestException("Passwords do not match.");
            }

            var emailOtp = await _authRepository.GetEmailOtpAsync(request.Email);
            if (emailOtp == null || emailOtp.OtpCode != request.Otp)
            {
                throw new BadRequestException("Invalid OTP.");
            }

            if (emailOtp.ExpiresAt < DateTime.UtcNow)
            {
                throw new BadRequestException("OTP has expired.");
            }

            emailOtp.IsUsed = true;
            await _authRepository.UpdateEmailOtpAsync(emailOtp);

            var user = emailOtp.User;
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);

            await _authRepository.UpdateUserAsync(user);
            await _authRepository.SaveChangesAsync();

            return ApiResponse<string>.SuccessResponse("Password has been reset successfully.", "Success", 200);
        }

        public async Task<ApiResponse<string>> LogoutAsync(string refreshToken)
        {
            var storedToken = await _authRepository.GetRefreshTokenAsync(refreshToken);

            if (storedToken != null && !storedToken.IsRevoked)
            {
                storedToken.IsRevoked = true;
                await _authRepository.UpdateRefreshTokenAsync(storedToken);
                await _authRepository.SaveChangesAsync();
            }

            return ApiResponse<string>.SuccessResponse("Logged out successfully.", "Success", 200);
        }
    }



    public class TempUserRegistration
    {
        public RegisterRequestDto Request { get; set; } = null!;
        public string PasswordHash { get; set; } = string.Empty;
        public string OtpCode { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
    }
}
