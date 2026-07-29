using Microsoft.EntityFrameworkCore;
using Worklance.Application.Interfaces.AuthInterface;
using Worklance.Domain.Entities.AuthEntities;
using Worklance.Infrastructure.Repositories;

namespace Worklance.Infrastructure.Data.Repositories.AuthRepo;

public class AuthRepository : GenericRepository<User>, IAuthRepository
{
    private new readonly AppDbContext _context;

    public AuthRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        return await _context.Users.AnyAsync(x => x.Email == email);
    }

        public async Task<bool> PhoneNumberExistsAsync(long phoneNumber)
        {
            return await _context.Users.AnyAsync(x => x.PhoneNumber == phoneNumber);
        }

        public async Task<bool> AadhaarNumberExistsAsync(long aadhaarNumber)
        {
            return await _context.Users.AnyAsync(x => x.AadhaarNumber == aadhaarNumber);
        }

    public async Task AddUserAsync(User user)
    {
        await _context.Users.AddAsync(user);
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await _context.Users.FirstOrDefaultAsync(x => x.Email == email);
    }

    public Task UpdateUserAsync(User user)
    {
        _context.Users.Update(user);
        return Task.CompletedTask;
    }

    public async Task AddEmailOtpAsync(EmailOtp emailOtp)
    {
        await _context.EmailOtps.AddAsync(emailOtp);
    }

    public async Task<EmailOtp?> GetEmailOtpAsync(string email)
    {
        return await _context.EmailOtps
            .Include(x => x.User)
            .OrderByDescending(x => x.Id)
            .FirstOrDefaultAsync(x => x.User.Email == email && !x.IsUsed);
    }

    public Task UpdateEmailOtpAsync(EmailOtp emailOtp)
    {
        _context.EmailOtps.Update(emailOtp);
        return Task.CompletedTask;
    }

    public Task DeleteEmailOtpAsync(EmailOtp emailOtp)
    {
        _context.EmailOtps.Remove(emailOtp);
        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task AddRefreshTokenAsync(RefreshToken refreshToken)
    {
        await _context.RefreshTokens.AddAsync(refreshToken);
    }

    public async Task<RefreshToken?> GetRefreshTokenAsync(string token)
    {
        return await _context.RefreshTokens
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.Token == token);
    }

    public Task UpdateRefreshTokenAsync(RefreshToken refreshToken)
    {
        _context.RefreshTokens.Update(refreshToken);
        return Task.CompletedTask;
    }

    public Task DeleteRefreshTokenAsync(RefreshToken refreshToken)
    {
        _context.RefreshTokens.Remove(refreshToken);
        return Task.CompletedTask;
    }
}
