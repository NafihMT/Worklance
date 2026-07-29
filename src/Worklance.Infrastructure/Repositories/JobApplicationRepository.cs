using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Worklance.Application.Interfaces.Repositories;
using Worklance.Domain.Entities.Job;
using Worklance.Infrastructure.Data;

namespace Worklance.Infrastructure.Repositories;

public class JobApplicationRepository : GenericRepository<JobApplication>, IJobApplicationRepository
{
    private new readonly AppDbContext _context;

    public JobApplicationRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<bool> HasAlreadyAppliedAsync(int jobId, int freelancerProfileId)
    {
        return await _context.JobApplications
            .AnyAsync(a => a.JobId == jobId && a.FreelancerProfileId == freelancerProfileId);
    }

    public async Task<JobApplication?> GetByIdWithDetailsAsync(int id)
    {
        return await _context.JobApplications
            .AsSplitQuery()
            .Include(a => a.Job)
                .ThenInclude(j => j.ClientProfile)
            .Include(a => a.FreelancerProfile)
                .ThenInclude(f => f.Skills)
            .Include(a => a.FreelancerProfile)
                .ThenInclude(f => f.Educations)
            .Include(a => a.FreelancerProfile)
                .ThenInclude(f => f.Certifications)
            .Include(a => a.FreelancerProfile)
                .ThenInclude(f => f.Portfolios)
            .Include(a => a.FreelancerProfile)
                .ThenInclude(f => f.Languages)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<IReadOnlyList<JobApplication>> GetApplicationsByJobIdAsync(int jobId)
    {
        return await _context.JobApplications
            .AsSplitQuery()
            .Include(a => a.Job)
                .ThenInclude(j => j.ClientProfile)
            .Include(a => a.FreelancerProfile)
                .ThenInclude(f => f.Skills)
            .Include(a => a.FreelancerProfile)
                .ThenInclude(f => f.Educations)
            .Include(a => a.FreelancerProfile)
                .ThenInclude(f => f.Certifications)
            .Include(a => a.FreelancerProfile)
                .ThenInclude(f => f.Portfolios)
            .Include(a => a.FreelancerProfile)
                .ThenInclude(f => f.Languages)
            .Where(a => a.JobId == jobId)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<JobApplication>> GetApplicationsByFreelancerIdAsync(int freelancerProfileId)
    {
        return await _context.JobApplications
            .Include(a => a.Job)
            .Include(a => a.FreelancerProfile)
            .Where(a => a.FreelancerProfileId == freelancerProfileId)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<JobApplication>> GetByUserIdAsync(string userId)
    {
        return await _context.JobApplications
            .Include(a => a.Job)
            .Include(a => a.FreelancerProfile)
            .Where(a => a.FreelancerProfile.UserId == userId)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();
    }
}
