using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Worklance.Application.Interfaces.Repositories;
using Worklance.Domain.Entities;
using Worklance.Infrastructure.Data;

namespace Worklance.Infrastructure.Repositories;

public class SkillRepository : GenericRepository<Skill>, ISkillRepository
{
    public SkillRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<Skill?> GetByNameAsync(string name)
    {
        var trimmedName = name?.Trim();
        if (string.IsNullOrEmpty(trimmedName)) return null;

        return await _context.Skills
            .Include(s => s.CategorySkills)
                .ThenInclude(cs => cs.Category)
            .FirstOrDefaultAsync(s => s.Name.ToLower() == trimmedName.ToLower() && !s.IsDeleted);
    }

    public async Task<Skill?> GetByIdWithCategoriesAsync(int id)
    {
        return await _context.Skills
            .Include(s => s.CategorySkills)
                .ThenInclude(cs => cs.Category)
            .FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted);
    }
}
