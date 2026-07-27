using Worklance.Domain.Common;
using Worklance.Domain.Enums.Job;

namespace Worklance.Domain.Entities.Job;

public class JobApplication : BaseAuditableEntity
{
    public int JobId { get; set; }
    public Job Job { get; set; } = null!;

    public int FreelancerProfileId { get; set; }
    public FreelancerProfile FreelancerProfile { get; set; } = null!;

    public string? CoverLetterText { get; set; }

    public string? CoverLetterFileUrl { get; set; }
    public string? CoverLetterFileName { get; set; }

    public decimal ProposedRate { get; set; }
    public int EstimatedDays { get; set; }
    public JobApplicationStatus Status { get; set; } = JobApplicationStatus.Pending;
}
