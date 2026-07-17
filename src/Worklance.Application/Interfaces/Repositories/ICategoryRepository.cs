using System.Collections.Generic;
using System.Threading.Tasks;
using Worklance.Domain.Entities.Job;

namespace Worklance.Application.Interfaces.Repositories;

public interface ICategoryRepository : IGenericRepository<Category>
{
    Task<Category?> GetByIdWithSkillsAsync(int id);
    Task<bool> ExistsByNameAsync(string name);
    Task<bool> ExistsByNameExcludingIdAsync(string name, int id);
    Task<IReadOnlyList<Category>> GetAllCategoriesWithSkillsAsync();
}
