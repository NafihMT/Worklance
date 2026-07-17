using Microsoft.EntityFrameworkCore;
using Worklance.Application.Interfaces.Repositories;
using Worklance.Domain.Entities;
using Worklance.Domain.Entities.Job;
using Worklance.Domain.Enums.Job;
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
            .AsNoTracking()
            .AsSplitQuery()
            .Where(x => x.Id == jobId && !x.IsDeleted)

            .Include(j => j.ClientProfile)
            .Include(j => j.Category)
            .Include(j => j.JobSkills)
                .ThenInclude(js => js.Skill)
            .FirstOrDefaultAsync();
    }

    public async Task<IReadOnlyList<Job>> GetAllJobsWithDetailsAsync()
    {
        return await _context.Jobs
            .AsNoTracking()
            .Where(j => !j.IsDeleted)

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

    public async Task<IReadOnlyList<int>> GetExistingSkillIdsAsync(List<int> skillIds, int categoryId)
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

    public async Task<Job?> GetJobForUpdateAsync(int jobId, string userId)
    {
        return await _context.Jobs
            .Include(j => j.JobSkills)
            .Include(j => j.ClientProfile)
            .FirstOrDefaultAsync(j =>
                j.Id == jobId &&
                !j.IsDeleted &&
                j.ClientProfile.UserId == userId);
    }


    public async Task<IReadOnlyList<Job>> GetJobsByClientProfileIdAsync(int clientProfileId, JobStatus? status)
    {
        // Filtering by Status
        var query = _context.Jobs
            .AsNoTracking()
            .AsSplitQuery()
            .Where(j => j.ClientProfileId == clientProfileId && !j.IsDeleted);

        if (status.HasValue)
        {
            query = query.Where(j => j.Status == status.Value);
        }

        return await query
            .Include(j => j.ClientProfile)
            .Include(j => j.Category)
            .Include(j => j.JobSkills)
                .ThenInclude(js => js.Skill)
            .OrderByDescending(j => j.CreatedAt)
            .ToListAsync();
    }

    public async Task<bool> CategoryNameExistsAsync(string categoryName)
    {
        return await _context.Categories
            .AsNoTracking()
            .AnyAsync(c =>
                c.Name.ToLower() == categoryName.Trim().ToLower()
                && !c.IsDeleted);
    }
}
