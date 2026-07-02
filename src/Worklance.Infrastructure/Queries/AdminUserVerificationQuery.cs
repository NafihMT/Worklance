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
                    Id AS UserId,
                    FullName,
                    Email,
                    PhoneNumber,
                    CASE 
                        WHEN AccountType = 1 THEN 'JobSeeker'
                        WHEN AccountType = 2 THEN 'JobRecruiter'
                        ELSE 'Unknown'
                    END AS AccountType,
                    AadhaarNumber,
                    AadhaarProofPath,
                    CreatedAt AS RegisteredAt
                FROM Users
                WHERE AdminVerificationStatus = 1
                ORDER BY CreatedAt ASC";

            using var connection = _context.CreateConnection();

            var result = await connection.QueryAsync<UserVerificationDTO>(sql);

            return result;
        }
    }
}