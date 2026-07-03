using Microsoft.EntityFrameworkCore;
using Worklance.Application.Interfaces;
using Worklance.Application.Interfaces.Repositories;
using Worklance.Domain.Entities;
using Worklance.Domain.Entities.Job;
using Worklance.Infrastructure.Persistence;
using Worklance.Infrastructure.Data;

namespace Worklance.Infrastructure.Repositories;

public class JobRepository : GenericRepository<Job>, IJobRepository
{
    public JobRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<Job?> GetJobWithDetailsAsync(int jobId)
    {
        return await _context.Jobs
            .Include(j => j.ClientProfile)
            .Include(j => j.Category)
            .Include(j => j.JobSkills)
                .ThenInclude(js => js.Skill)
            .FirstOrDefaultAsync(j => j.JobId == jobId);
    }

    public async Task<IReadOnlyList<Job>> GetAllJobsWithDetailsAsync()
    {
        return await _context.Jobs
            .Include(j => j.ClientProfile)
            .Include(j => j.Category)
            .Include(j => j.JobSkills)
                .ThenInclude(js => js.Skill)
            .OrderByDescending(j => j.CreatedAt)
            .ToListAsync();
    }

    public async Task<bool> CategoryExistsAsync(int categoryId)
    {
        return await _context.Categories.AnyAsync(c => c.Id == categoryId);
    }

    public async Task<List<int>> GetExistingSkillIdsAsync(List<int> skillIds)
    {
        return await _context.Skills
            .Where(s => skillIds.Contains(s.Id))
            .Select(s => s.Id)
            .ToListAsync();
    }

    public async Task<int> GetClientProfileIdByUserIdAsync(string userId)
    {
        var profile = await _context.FreelancerProfiles
            .FirstOrDefaultAsync(cp => cp.UserId == userId);

        return profile?.Id ?? 0;
    }
}