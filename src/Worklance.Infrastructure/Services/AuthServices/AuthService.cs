using FluentValidation;
using System;
using System.Linq;
using System.Threading.Tasks;
using Worklance.Application.Common.ApiResponse;
using Worklance.Application.Common.OTP;
using Worklance.Application.DTOs.AuthDTOs;
using Worklance.Application.DTOs.ResendOtpRequestDto;
using Worklance.Application.Exceptions;
using Worklance.Application.Interfaces.AuthInterface;
using Worklance.Application.Interfaces.EmailInterface;
using Worklance.Domain.Entities.AuthEntities;
using Worklance.Domain.Enums.AuthEnums;
using Worklance.Infrastructure.Data.Repositories.AuthRepo;

namespace Worklance.Infrastructure.Services.AuthServices
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly IEmailService _emailService;
        private readonly IOcrService _ocrService;
        private readonly IJwtService _jwtService;
        private readonly IValidator<RegisterRequestDto> _registerValidator;
        private readonly IValidator<LoginRequestDto> _loginValidator;

        public AuthService(
            IAuthRepository authRepository,
            IEmailService emailService,
            IOcrService ocrService,
            IJwtService jwtService,
            IValidator<RegisterRequestDto> registerValidator,
            IValidator<LoginRequestDto> loginValidator)
        {
            _authRepository = authRepository;
            _emailService = emailService;
            _ocrService = ocrService;
            _jwtService = jwtService;
            _registerValidator = registerValidator;
            _loginValidator = loginValidator;
        }

        public async Task<ApiResponse> RegisterAsync(RegisterRequestDto request)
        {
            var validationResult = await _registerValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                var errorMessages = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                if (errorMessages.Count == 1) return ApiResponse.Failure(errorMessages.First(), 400);
                return ApiResponse.Failure("Registration failed due to multiple validation errors.", 400, errorMessages);
            }

            await ValidateDuplicateAsync(request);

            bool ocrResult = await VerifyAadhaarAsync(Convert.ToBase64String(request.AadhaarImageBytes), request.AadhaarNumber);
            var passwordHash = HashPassword(request.Password);

            var user = CreateUser(request, passwordHash, request.AadhaarImageBytes, ocrResult);
            await _authRepository.AddUserAsync(user);
            await _authRepository.SaveChangesAsync();

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

            await _emailService.SendOtpAsync(request.Email, otp);

            return ApiResponse.Success("Registration successful. OTP has been sent to your email.", 201);
        }

        public async Task<ApiResponse> VerifyOtpAsync(VerifyOtpRequestDto request)
        {
            if (request == null || string.IsNullOrEmpty(request.Email))
            {
                throw new BadRequestException("Email is required for verification.");
            }

            var user = await _authRepository.GetUserByEmailAsync(request.Email.Trim());

            if (user == null)
            {
                throw new BadRequestException("User not found.");
            }

            if (user.EmailVerified)
            {
                throw new BadRequestException("Email is already verified. You can log in.");
            }

            var emailOtp = await _authRepository.GetEmailOtpAsync(request.Email.Trim());

            if (emailOtp == null || emailOtp.OtpCode != request.Otp.Trim())
                throw new BadRequestException("Invalid OTP.");

            if (emailOtp.ExpiresAt < DateTime.UtcNow)
                throw new BadRequestException("OTP has expired.");

            if (emailOtp.IsUsed)
                throw new BadRequestException("This OTP has already been used.");

            emailOtp.IsUsed = true;
            await _authRepository.UpdateEmailOtpAsync(emailOtp);

            user.EmailVerified = true;
            await _authRepository.UpdateUserAsync(user);

            await _authRepository.SaveChangesAsync();

            return ApiResponse.Success(null, "Email verified successfully! You can login now.", 200);
        }

        public async Task<ApiResponse> LoginAsync(LoginRequestDto request)
        {
            var validationResult = await _loginValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                var errorMessages = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                if (errorMessages.Count == 1) return ApiResponse.Failure(errorMessages.First(), 400);
                return ApiResponse.Failure("Login failed due to multiple validation errors.", 400, errorMessages);
            }

            var user = await _authRepository.GetUserByEmailAsync(request.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                throw new BadRequestException("Invalid email or password.");

            if (!user.EmailVerified)
                throw new BadRequestException("Please verify your email first.");

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

            return ApiResponse.Success(response, "Login successful.", 200);
        }

        public async Task<ApiResponse> RefreshTokenAsync(RefreshTokenRequestDto request)
        {
            var storedToken = await _authRepository.GetRefreshTokenAsync(request.RefreshToken);
            if (storedToken == null || storedToken.IsRevoked || storedToken.ExpiresAt < DateTime.UtcNow)
                throw new BadRequestException("Invalid or expired refresh token.");

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

            return ApiResponse.Success(response, "Token refreshed successfully.", 200);
        }

        public async Task<ApiResponse> ForgotPasswordAsync(ForgotPasswordRequestDto request)
        {
            var user = await _authRepository.GetUserByEmailAsync(request.Email);
            if (user == null)
                throw new BadRequestException("User with this email does not exist.");

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

            return ApiResponse.Success("Password reset OTP has been sent to your email.", 200);
        }

        public async Task<ApiResponse> ResetPasswordAsync(ResetPasswordRequestDto request)
        {
            if (request.NewPassword != request.ConfirmPassword)
                throw new BadRequestException("Passwords do not match.");

            var emailOtp = await _authRepository.GetEmailOtpAsync(request.Email);
            if (emailOtp == null || emailOtp.OtpCode != request.Otp)
                throw new BadRequestException("Invalid OTP.");

            if (emailOtp.ExpiresAt < DateTime.UtcNow)
                throw new BadRequestException("OTP has expired.");

            emailOtp.IsUsed = true;
            await _authRepository.UpdateEmailOtpAsync(emailOtp);

            var user = await _authRepository.GetUserByEmailAsync(request.Email);
            if (user != null)
            {
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
                await _authRepository.UpdateUserAsync(user);
            }
            await _authRepository.SaveChangesAsync();

            return ApiResponse.Success("Password has been reset successfully.", 200);
        }

        public async Task<ApiResponse> LogoutAsync(string refreshToken)
        {
            var storedToken = await _authRepository.GetRefreshTokenAsync(refreshToken);

            if (storedToken != null && !storedToken.IsRevoked)
            {
                storedToken.IsRevoked = true;
                await _authRepository.UpdateRefreshTokenAsync(storedToken);
                await _authRepository.SaveChangesAsync();
            }

            return ApiResponse.Success("Logged out successfully.", 200);
        }


        private async Task ValidateDuplicateAsync(RegisterRequestDto request)
        {
            if (await _authRepository.EmailExistsAsync(request.Email) ||
                await _authRepository.PhoneNumberExistsAsync(request.PhoneNumber) ||
                await _authRepository.AadhaarNumberExistsAsync(request.AadhaarNumber))
            {
                throw new BadRequestException("User registered already.");
            }
        }

        private async Task<bool> VerifyAadhaarAsync(string imageUrl, string aadhaarNumber)
        {
            var extractedAadhaar = await _ocrService.ReadAadhaarNumberAsync(imageUrl);
            if (string.IsNullOrEmpty(extractedAadhaar) || extractedAadhaar != aadhaarNumber)
            {
                return false;
            }
            return true;
        }

        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        private User CreateUser(RegisterRequestDto request, string passwordHash, byte[] imageBytes, bool isOcrMatched)
        {
            return new User
            {
                FullName = request.FullName,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                PasswordHash = passwordHash,
                AadhaarNumber = request.AadhaarNumber,
                AadhaarImageBytes = imageBytes, 
                IsOcrMatched = isOcrMatched,
                AccountType = request.AccountType,
                EmailVerified = false,
                Status = UserStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };
        }

        public async Task<ApiResponse> ResendOtpAsync(ResendOtpRequestDto request)
        {
            var user = await _authRepository.GetUserByEmailAsync(request.Email.Trim());
            if (user == null)
                throw new BadRequestException("User not found.");

            if (user.EmailVerified)
                throw new BadRequestException("Email is already verified. You can log in.");

            var lastOtp = await _authRepository.GetEmailOtpAsync(request.Email.Trim());

            if (lastOtp != null && lastOtp.ExpiresAt > DateTime.UtcNow.AddMinutes(4))
            {
                throw new BadRequestException("Please wait 60 seconds before requesting a new OTP.");
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

            return ApiResponse.Success(null, "A new OTP has been sent to your email.", 200);
        }
    }
}