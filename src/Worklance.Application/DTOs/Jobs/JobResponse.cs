using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Worklance.Domain.Enums.Job;

namespace Worklance.Application.DTOs.Jobs
{
    public class JobResponse
    {
        public int JobId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public int ClientProfileId { get; set; }
        public string ClientName { get; set; } = string.Empty;

        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;

        public JobType JobType { get; set; }
        public JobStatus Status { get; set; }

        public decimal? FixedBudget { get; set; }
        public decimal? MinHourlyRate { get; set; }
        public decimal? MaxHourlyRate { get; set; }

        public DateTime Deadline { get; set; }

        public List<string> Tags { get; set; } = new();
        public List<string> Skills { get; set; } = new();

        public string? AttachmentUrl { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
