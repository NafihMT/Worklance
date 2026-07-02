using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Worklance.Domain.Entities.Job;

namespace Worklance.Application.Interfaces.Repositories
{
    public interface IJobRepository : IGenericRepository<Job>
    {
        Task<Job?> GetJobWithDetailsAsync(int jobId); 
        Task<IReadOnlyList<Job>> GetAllJobsWithDetailsAsync();
        Task<bool> CategoryExistsAsync(int categoryId);
        Task<List<int>> GetExistingSkillIdsAsync(List<int> skillIds);
        Task<int> GetClientProfileIdByUserIdAsync(string userId);
    }
}
