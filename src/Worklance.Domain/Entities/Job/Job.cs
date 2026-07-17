using Worklance.Domain.Common;
using Worklance.Domain.Enums.Job;

namespace Worklance.Domain.Entities.Job
{
    public class Job : BaseAuditableEntity
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int ClientProfileId { get; set; }
        public FreelancerProfile ClientProfile { get; set; } = null!;

        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;
 

        public JobType JobType { get; set; }
        public JobStatus Status { get; set; } = JobStatus.Open;

        public decimal? FixedBudget { get; set; }
        public decimal? MinHourlyRate { get; set; }
        public decimal? MaxHourlyRate { get; set; }

        public DateTime Deadline { get; set; }

        public string Tags { get; set; } = "[]";

        public byte[]? AttachmentBytes { get; set; }

        public ICollection<JobSkill> JobSkills { get; set; } = new List<JobSkill>();
        public ICollection<Bid> Bids { get; set; } = new List<Bid>();

        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }

    }
}
