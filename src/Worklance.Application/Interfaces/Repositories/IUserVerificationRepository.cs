using System.Threading.Tasks;
using Worklance.Domain.Enums;

namespace Worklance.Application.Interfaces.Repositories
{
    public interface IUserVerificationRepository
    {
        Task<(bool IsSuccess, string? ErrorMessage)> UpdateStatusAsync(string userId, 
            VerificationStatus status, string? reason);
    }
}