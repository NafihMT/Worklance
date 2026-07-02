using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Worklance.Domain.Enums.Job;

namespace Worklance.Domain.Entities.Job
{
    public class Job
    {
        public int JobId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int ClientProfileId { get; set; }
        public FreelancerProfile ClientProfile { get; set; } = null!;

        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;
 

        public JobType JobType { get; set; }
        public JobStatus Status { get; set; } = JobStatus.Open;

        // Budget fields — usage depends on JobType
        public decimal? FixedBudget { get; set; }
        public decimal? MinHourlyRate { get; set; }
        public decimal? MaxHourlyRate { get; set; }

        public DateTime Deadline { get; set; }

        // Stored as JSON string in DB, exposed as List<string> in Application layer
        public string TagsJson { get; set; } = "[]";

        public string? AttachmentUrl { get; set; }

        public ICollection<JobSkill> JobSkills { get; set; } = new List<JobSkill>();
        public ICollection<Bid> Bids { get; set; } = new List<Bid>();

        // Soft delete
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }

        // Audit
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
