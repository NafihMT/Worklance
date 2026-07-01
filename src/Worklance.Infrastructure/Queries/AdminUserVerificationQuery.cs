using Dapper;
using Worklance.Application.DTOs.AdminUserDTO;
using Worklance.Application.Interfaces.Queries;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Worklance.Infrastructure.Queries
{
    public class AdminUserVerificationQuery : IAdminUserVerificationQuery
    {
        private readonly DapperContext _context;

        public AdminUserVerificationQuery(DapperContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<UserVerificationDTO>> GetPendingVerificationsAsync()
        {
            var sql = @"
                SELECT 
                    u.Id AS UserId,
                    u.FullName,
                    u.Email,
                    u.AccountType,
                    
                    p.ProfilePhotoUrl,
                    p.Profession,
                    p.Headline,
                    p.Bio,
                    p.Location,
                    
                    p.GitHubUrl,
                    p.BehanceUrl,
                    p.PortfolioUrl,
                    p.BusinessName,
                    p.Services,
                    p.LinkedInUrl,
                    
                    v.AadhaarNumber,
                    v.AadhaarFrontImageUrl,
                    v.AadhaarBackImageUrl,
                    v.VerificationStatus
                    
                FROM Users u
                LEFT JOIN Profiles p ON u.Id = p.UserId
                INNER JOIN IdentityVerifications v ON u.Id = v.UserId
                
                WHERE v.VerificationStatus = 'Pending'
                ORDER BY v.CreatedAt ASC";

            using var connection = _context.CreateConnection();

            var result = await connection.QueryAsync<UserVerificationDTO>(sql);

            return result;
        }
    }
}