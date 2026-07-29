using System.Collections.Generic;
using System.Threading.Tasks;
using Worklance.Domain.Entities.Job;

namespace Worklance.Application.Interfaces.Repositories;

public interface IJobApplicationRepository : IGenericRepository<JobApplication>
{
    Task<bool> HasAlreadyAppliedAsync(int jobId, int freelancerProfileId);
    Task<JobApplication?> GetByIdWithDetailsAsync(int id);
    Task<IReadOnlyList<JobApplication>> GetApplicationsByJobIdAsync(int jobId);
    Task<IReadOnlyList<JobApplication>> GetApplicationsByFreelancerIdAsync(int freelancerProfileId);
    Task<IReadOnlyList<JobApplication>> GetByUserIdAsync(string userId);
}
