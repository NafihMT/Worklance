using System.Threading.Tasks;
using Worklance.Domain.Entities;

namespace Worklance.Application.Interfaces.Repositories;

public interface IFreelancerProfileRepository : IGenericRepository<FreelancerProfile>
{
    Task<FreelancerProfile?> GetProfileWithDetailsAsync(string userId);
    Task<FreelancerProfile?> GetProfileWithDetailsByIdAsync(int id);
    Task<bool> HasProfileAsync(string userId);
}
