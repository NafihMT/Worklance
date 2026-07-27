using System;
using Worklance.Application.DTOs.FreelancerProfiles;
using Worklance.Domain.Enums.Job;

namespace Worklance.Application.DTOs.JobApplications;

public class JobApplicationResponse
{
    public int Id { get; set; }
    public int JobId { get; set; }
    public string JobTitle { get; set; } = string.Empty;
    public int FreelancerProfileId { get; set; }
    public string FreelancerName { get; set; } = string.Empty;
    public string? FreelancerTitle { get; set; }
    public string? FreelancerPhotoUrl { get; set; }
    public string? FreelancerEmail { get; set; }

    public FreelancerProfileDto? ApplicantProfile { get; set; }

    public string? CoverLetterText { get; set; }
    public string? CoverLetterFileUrl { get; set; }
    public string? CoverLetterFileName { get; set; }
    public decimal ProposedRate { get; set; }
    public int EstimatedDays { get; set; }
    public JobApplicationStatus Status { get; set; }
    public DateTime AppliedAt { get; set; }
}
