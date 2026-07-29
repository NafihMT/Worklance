using System.Collections.Generic;
using System.Threading.Tasks;
using Worklance.Application.DTOs.AdminUserDTO;

namespace Worklance.Application.Interfaces.Queries
{
    public interface IAdminUserVerificationQuery
    {
        Task<IEnumerable<UserVerificationDTO>> GetPendingVerificationsAsync();

        Task<(string FullName, byte[] ImageBytes)?> GetUserAadhaarImageAsync(string userId);
    }
}