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
        CreatedAt AS RegisteredAt,
        CASE 
            WHEN IsOcrMatched = 1 THEN 'Matched'
            ELSE 'Mismatch'
        END AS OcrStatus   
        FROM Users
        -- STRICT VALIDATION: Only show Pending Admin verification AND Verified Emails!
        WHERE AdminVerificationStatus = 1 AND EmailVerified = 1
        -- Put the newest users at the top!
        ORDER BY CreatedAt DESC";

            using var connection = _context.CreateConnection();

            var result = await connection.QueryAsync<UserVerificationDTO>(sql);

            return result;
        }

        public async Task<(string FullName, byte[] ImageBytes)?> GetUserAadhaarImageAsync(string userId)
        {
            if (!int.TryParse(userId, out int parsedId)) return null;

            var sql = @"
        SELECT FullName, AadhaarImageBytes 
        FROM Users 
        WHERE Id = @Id";

            using var connection = _context.CreateConnection();
            var result = await connection.QuerySingleOrDefaultAsync<dynamic>(sql, new { Id = parsedId });

            if (result == null) return null;

            return (result.FullName, (byte[])result.AadhaarImageBytes);
        }
    }
}