using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Worklance.Domain.Entities.AuthEntities;

namespace Worklance.Application.Interfaces.AuthInterface
{
    public interface IAuthRepository
    {
        // Duplicate Validation
        Task<bool> EmailExistsAsync(string email);
        Task<bool> PhoneNumberExistsAsync(long phoneNumber);
        Task<bool> AadhaarNumberExistsAsync(long aadhaarNumber);

        // User
        Task AddUserAsync(User user);
        Task<User?> GetUserByEmailAsync(string email);
        Task UpdateUserAsync(User user);

        // Email OTP
        Task AddEmailOtpAsync(EmailOtp emailOtp);
        Task<EmailOtp?> GetEmailOtpAsync(string email);
        Task UpdateEmailOtpAsync(EmailOtp emailOtp);
        Task DeleteEmailOtpAsync(EmailOtp emailOtp);

        // Refresh Token
        Task AddRefreshTokenAsync(RefreshToken refreshToken);
        Task<RefreshToken?> GetRefreshTokenAsync(string token);
        Task UpdateRefreshTokenAsync(RefreshToken refreshToken);
        Task DeleteRefreshTokenAsync(RefreshToken refreshToken);

        // Save Changes
        Task SaveChangesAsync();
    }
}
