using Microsoft.EntityFrameworkCore;
using Worklance.Application.Interfaces.Repositories;
using Worklance.Domain.Entities;
using Worklance.Domain.Entities.Job;
using Worklance.Infrastructure.Data;

namespace Worklance.Infrastructure.Repositories;

public class JobRepository : GenericRepository<Job>, IJobRepository
{
    public JobRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<Job?> GetJobWithDetailsAsync(int jobId)
    {
        if (jobId <= 0)
            return null;

        return await _context.Jobs
            .Where(x => x.Id == jobId && !x.IsDeleted)
            .AsNoTracking()
            .AsSplitQuery()

            .Include(j => j.ClientProfile)
            .Include(j => j.Category)
            .Include(j => j.JobSkills)
                .ThenInclude(js => js.Skill)
            .FirstOrDefaultAsync();
    }

    public async Task<IReadOnlyList<Job>> GetAllJobsWithDetailsAsync()
    {
        return await _context.Jobs
            .Where(j => !j.IsDeleted)
            .AsNoTracking()
            .AsSplitQuery()

            .Include(j => j.ClientProfile)
            .Include(j => j.Category)
            .Include(j => j.JobSkills)
                .ThenInclude(js => js.Skill)
            .OrderByDescending(j => j.CreatedAt)
            .ToListAsync();
    }

    public async Task<bool> CategoryExistsAsync(int categoryId)
    {
        return await _context.Categories.AnyAsync(c => c.Id == categoryId && !c.IsDeleted);
    }

    public async Task<IReadOnlyList<Category>> GetAllCategoriesAsync()
    {
        return await _context.Categories
            .AsNoTracking()
            .Where(c => !c.IsDeleted)
            .Include(c => c.Skills)
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Skill>> GetSkillsByCategoryIdAsync(int categoryId)
    {
        return await _context.Skills
            .AsNoTracking()
            .Where(s => s.CategoryId == categoryId)
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<List<int>> GetExistingSkillIdsAsync(List<int> skillIds, int categoryId)
    {
        return await _context.Skills
            .AsNoTracking()
            .Where(s => skillIds.Contains(s.Id) && s.CategoryId == categoryId)
            .Select(s => s.Id)
            .ToListAsync();
    }

    public async Task<int> GetClientProfileIdByUserIdAsync(string userId)
    {
        return await _context.FreelancerProfiles
            .AsNoTracking()
            .Where(cp => cp.UserId == userId)
            .Select(cp => cp.Id)
            .FirstOrDefaultAsync();
    }
}