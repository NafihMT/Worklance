using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Worklance.Application.Interfaces.Repositories;
using Worklance.Domain.Entities.Job;
using Worklance.Infrastructure.Data;

namespace Worklance.Infrastructure.Repositories;

public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
{
    public CategoryRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<Category?> GetByIdWithSkillsAsync(int id)
    {
        return await _context.Categories
            .Include(c => c.CategorySkills)
                .ThenInclude(cs => cs.Skill)
            .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
    }

    public async Task<bool> ExistsByNameAsync(string name)
    {
        var trimmedName = name?.Trim().ToLower();
        if (string.IsNullOrEmpty(trimmedName)) return false;

        return await _context.Categories
            .AnyAsync(c => c.Name.ToLower() == trimmedName && !c.IsDeleted);
    }

    public async Task<bool> ExistsByNameExcludingIdAsync(string name, int id)
    {
        var trimmedName = name?.Trim().ToLower();
        if (string.IsNullOrEmpty(trimmedName)) return false;

        return await _context.Categories
            .AnyAsync(c => c.Name.ToLower() == trimmedName && c.Id != id && !c.IsDeleted);
    }

    public async Task<IReadOnlyList<Category>> GetAllCategoriesWithSkillsAsync()
    {
        return await _context.Categories
            .Include(c => c.CategorySkills)
                .ThenInclude(cs => cs.Skill)
            .Where(c => !c.IsDeleted)
            .OrderBy(c => c.Name)
            .ToListAsync();
    }
}
