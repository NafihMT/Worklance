using Worklance.Application.DTOs.Jobs;

namespace Worklance.Application.Interfaces.Services
{
    public interface IJobService
    {
        Task<JobResponse> CreateJobAsync(string userId, CreateJobRequest request);
        Task<JobResponse> UpdateJobAsync(int jobId, string userId, UpdateJobRequest request);
        Task<IEnumerable<JobResponse>> GetAllJobsAsync();
        Task<JobResponse> GetJobByIdAsync(int jobId);
        Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync();
        Task<IEnumerable<CategorySkillDto>> GetSkillsByCategoryIdAsync(int categoryId);

        Task SoftDeleteJobAsync(int jobId, string userId);
        Task CloseJobAsync(int jobId, string userId);
        Task ReopenJobAsync(int jobId, string userId);
        Task CancelJobAsync(int jobId, string userId);
    }
}
