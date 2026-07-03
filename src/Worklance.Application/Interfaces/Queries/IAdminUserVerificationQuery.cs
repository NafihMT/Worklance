using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Worklance.Application.DTOs.AdminUserDTO;

namespace Worklance.Application.Interfaces.Queries
{
    public interface IAdminUserVerificationQuery
    {
        Task<IEnumerable<UserVerificationDTO>> GetPendingVerificationsAsync();
    }
}
