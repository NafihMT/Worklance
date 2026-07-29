using System;
using Worklance.Application.Interfaces.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Worklance.Infrastructure.Repositories
{
    public class UserVerificationRepository : IUserVerificationRepository
    {
        private readonly Worklance.Infrastructure.Data.AppDbContext _context;

        public UserVerificationRepository(Worklance.Infrastructure.Data.AppDbContext context)
        {
            _context = context;
        }

        public async Task<(bool IsSuccess, string? ErrorMessage)> UpdateStatusAsync(string userId, Worklance.Domain.Enums.VerificationStatus status, string? reason)
        {
            if (!int.TryParse(userId, out int parsedUserId))
                return (false, "Invalid user ID format.");

            var user = await _context.Users.FindAsync(parsedUserId);
            if (user == null)
                return (false, $"Verification record for User {userId} not found.");

            if (user.Role == Worklance.Domain.Enums.AuthEnums.UserRole.Admin)
            {
                return (false, "Action Denied: You cannot update the verification status of another Admin.");
            }

            if (status == Worklance.Domain.Enums.VerificationStatus.Approved)
            {
                user.AdminVerificationStatus = Worklance.Domain.Enums.AuthEnums.AdminVerificationStatus.Approved;
                user.Status = Worklance.Domain.Enums.AuthEnums.UserStatus.Approved;
                user.RejectionReason = null;
            }
            else if (status == Worklance.Domain.Enums.VerificationStatus.Rejected)
            {
                user.AdminVerificationStatus = Worklance.Domain.Enums.AuthEnums.AdminVerificationStatus.Rejected;
                user.Status = Worklance.Domain.Enums.AuthEnums.UserStatus.Rejected;
                user.RejectionReason = reason;
            }

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return (true, null);    
        }
    }
}
