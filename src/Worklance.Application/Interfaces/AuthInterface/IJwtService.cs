using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Worklance.Domain.Entities.AuthEntities;

namespace Worklance.Application.Interfaces.AuthInterface
{
    public interface IJwtService
    {
        string GenerateAccessToken(User user);

        RefreshToken GenerateRefreshToken();
    }
}
