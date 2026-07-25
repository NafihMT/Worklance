using System.Threading.Tasks;
using Worklance.Application.Interfaces.Repositories;
using Worklance.Domain.Entities.AuthEntities;

namespace Worklance.Application.Interfaces.AuthInterface;

public interface IAuthRepository : IGenericRepository<User>
{
    // Duplicate Validation
    Task<bool> EmailExistsAsync(string email);
    Task<bool> PhoneNumberExistsAsync(string phoneNumber);
    Task<bool> AadhaarNumberExistsAsync(string aadhaarNumber);

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
