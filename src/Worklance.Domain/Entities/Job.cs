using Worklance.Domain.Common;
using Worklance.Domain.Enums;

namespace Worklance.Domain.Entities;

public class Job : BaseAuditableEntity
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Budget { get; set; }
    public JobStatus Status { get; set; } = JobStatus.Draft;
    public string ClientId { get; set; } = string.Empty;
    public string? FreelancerId { get; set; }
}
