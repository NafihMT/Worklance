using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Worklance.Application.DTOs.Jobs;

namespace Worklance.Application.Interfaces.Services
{
    public interface IJobService
    {
        Task<JobResponse> CreateJobAsync(string userId, CreateJobRequest request);
        Task<IEnumerable<JobResponse>> GetAllJobsAsync();
        Task<JobResponse> GetJobByIdAsync(int jobId);
        Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync();
        Task<IEnumerable<CategorySkillDto>> GetSkillsByCategoryIdAsync(int categoryId);
    }
}
