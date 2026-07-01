using System;

namespace Worklance.Application.DTOs.FreelancerProfiles;

public class FreelancerPortfolioDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? ProjectUrl { get; set; }
    public string? ImageUrl { get; set; }
    public string? TechnologiesUsed { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}
