using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using Worklance.Domain.Enums.Job;

namespace Worklance.Application.DTOs.Jobs
{
    public class JobRequestBase
    {
        [Required, MaxLength(150)]
        public string Title { get; set; } = string.Empty;

        [Required, MaxLength(5000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public int CategoryId { get; set; }

        [Required]
        public JobType JobType { get; set; }

        //Required only when JobType = Contract
        public decimal? FixedBudget { get; set; }

        //Required only when JobType = Hourly
        public decimal? MinHourlyRate { get; set; }
        public decimal? MaxHourlyRate { get; set; }

        [Required]
        public DateTime Deadline { get; set; }

        public List<int> SkillIds { get; set; } = new();

        public List<string> Tags { get; set; } = new();

        public IFormFile? AttachmentUrl { get; set; }
    }

    public class CreateJobRequest : JobRequestBase
    {
    }
    public class UpdateJobRequest : JobRequestBase
    {
    }


}
