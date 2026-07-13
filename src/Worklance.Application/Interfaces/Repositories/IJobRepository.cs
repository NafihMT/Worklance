using Worklance.Domain.Entities;
using Worklance.Domain.Entities.Job;
using Worklance.Domain.Enums.Job;

namespace Worklance.Application.Interfaces.Repositories
{
    public interface IJobRepository : IGenericRepository<Job>
    {
        Task<Job?> GetJobWithDetailsAsync(int jobId);
        Task<IReadOnlyList<Job>> GetAllJobsWithDetailsAsync();
        Task<bool> CategoryExistsAsync(int categoryId);
        Task<IReadOnlyList<Category>> GetAllCategoriesAsync();
        Task<IReadOnlyList<Skill>> GetSkillsByCategoryIdAsync(int categoryId);
        Task<List<int>> GetExistingSkillIdsAsync(List<int> skillIds, int categoryId);
        Task<int> GetClientProfileIdByUserIdAsync(string userId);

        Task<Job?> GetJobForUpdateAsync(int jobId);
        Task<bool> IsJobOwnedByClientAsync(int jobId, int clientProfileId);

        Task<IReadOnlyList<Job>> GetJobsByClientProfileIdAsync(int clientProfileId, JobStatus? status);
    }
}
