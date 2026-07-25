using System;
using Worklance.Domain.Enums.Job;

namespace Worklance.Application.DTOs.JobApplications;

public class JobApplicationResponse
{
    public int Id { get; set; }
    public int JobId { get; set; }
    public string JobTitle { get; set; } = string.Empty;
    public int FreelancerProfileId { get; set; }
    public string FreelancerName { get; set; } = string.Empty;
    public string CoverLetterFileName { get; set; } = string.Empty;
    public string CoverLetterContentType { get; set; } = string.Empty;
    public decimal ProposedRate { get; set; }
    public int EstimatedDays { get; set; }
    public JobApplicationStatus Status { get; set; }
    public DateTime AppliedAt { get; set; }
}
