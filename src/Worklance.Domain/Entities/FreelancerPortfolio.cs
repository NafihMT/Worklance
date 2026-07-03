using System;
using Worklance.Domain.Common;

namespace Worklance.Domain.Entities;

public class FreelancerPortfolio : BaseEntity
{
    public int FreelancerProfileId { get; set; }
    public FreelancerProfile FreelancerProfile { get; set; } = null!;
    
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? ProjectUrl { get; set; }
    public string? ImageUrl { get; set; }
    public string? TechnologiesUsed { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}
