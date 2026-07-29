using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Worklance.Application.Interfaces.Repositories;
using Worklance.Domain.Entities;
using Worklance.Infrastructure.Data;

namespace Worklance.Infrastructure.Repositories;

public class FreelancerProfileRepository : GenericRepository<FreelancerProfile>, IFreelancerProfileRepository
{
    public FreelancerProfileRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<FreelancerProfile?> GetProfileWithDetailsAsync(string userId)
    {
        return await _context.FreelancerProfiles
            .Include(fp => fp.Skills)
            .Include(fp => fp.Educations)
            .Include(fp => fp.Certifications)
            .Include(fp => fp.Portfolios)
            .Include(fp => fp.Languages)
            .FirstOrDefaultAsync(fp => fp.UserId == userId);
    }

    public async Task<FreelancerProfile?> GetProfileWithDetailsByIdAsync(int id)
    {
        return await _context.FreelancerProfiles
            .Include(fp => fp.Skills)
            .Include(fp => fp.Educations)
            .Include(fp => fp.Certifications)
            .Include(fp => fp.Portfolios)
            .Include(fp => fp.Languages)
            .FirstOrDefaultAsync(fp => fp.Id == id);
    }
    //test
    public async Task<bool> HasProfileAsync(string userId)
    {
        return await _context.FreelancerProfiles
            .AnyAsync(fp => fp.UserId == userId);
    }
}
