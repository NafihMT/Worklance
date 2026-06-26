using Microsoft.EntityFrameworkCore;

namespace Worklance.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    // Group members can define their DbSets here, for example:
    // DbSet<Job> Jobs { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
