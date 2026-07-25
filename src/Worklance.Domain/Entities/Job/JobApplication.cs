using Worklance.Domain.Common;
using Worklance.Domain.Enums.Job;

namespace Worklance.Domain.Entities.Job;

public class JobApplication : BaseAuditableEntity
{
    public int JobId { get; set; }
    public Job Job { get; set; } = null!;

    public int FreelancerProfileId { get; set; }
    public FreelancerProfile FreelancerProfile { get; set; } = null!;

    public byte[] CoverLetterBytes { get; set; } = Array.Empty<byte>();
    public string CoverLetterFileName { get; set; } = string.Empty;
    public string CoverLetterContentType { get; set; } = string.Empty;

    public decimal ProposedRate { get; set; }
    public int EstimatedDays { get; set; }
    public JobApplicationStatus Status { get; set; } = JobApplicationStatus.Pending;
}
