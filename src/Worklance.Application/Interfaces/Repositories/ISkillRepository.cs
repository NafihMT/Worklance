using System.Threading.Tasks;
using Worklance.Domain.Entities;

namespace Worklance.Application.Interfaces.Repositories;

public interface ISkillRepository : IGenericRepository<Skill>
{
    Task<Skill?> GetByNameAsync(string name);
}
