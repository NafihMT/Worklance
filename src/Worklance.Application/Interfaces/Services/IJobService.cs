using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Worklance.Application.DTOs.Jobs;

namespace Worklance.Application.Interfaces.Services
{
    public interface IJobService
    {
        Task<JobResponse> CreateJobAsync(string userId, CreateJobRequest request);
        Task<IEnumerable<JobResponse>> GetAllJobsAsync();
        Task<JobResponse> GetJobByIdAsync(int id);
    }
}
