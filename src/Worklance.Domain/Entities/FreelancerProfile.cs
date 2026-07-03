using System;
using System.Collections.Generic;
using Worklance.Domain.Common;
using Worklance.Domain.Enums;

namespace Worklance.Domain.Entities;

public class FreelancerProfile : BaseAuditableEntity
{
    public string UserId { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public Gender? Gender { get; set; }
    public string? ProfilePhotoUrl { get; set; }
    
    // Address Details
    public string? Country { get; set; }
    public string? State { get; set; }
    public string? City { get; set; }
    public string? Address { get; set; }

    // Professional Details
    public string ProfessionalTitle { get; set; } = string.Empty;
    public string AboutMe { get; set; } = string.Empty;
    public string PrimaryTechnologyStack { get; set; } = string.Empty;
    public string Specialization { get; set; } = string.Empty;
    public bool IsExperienced { get; set; }
    public int? ExperienceYears { get; set; }
    public int? ExperienceMonths { get; set; }
    public AvailabilityStatus Availability { get; set; } = AvailabilityStatus.NotAvailable;
    public WorkPreferenceType WorkPreference { get; set; } = WorkPreferenceType.Both;
    public decimal? HourlyRate { get; set; }

    // Documents & Links
    public string? ResumeUrl { get; set; }
    public string? GitHubUrl { get; set; }
    public string? LinkedInUrl { get; set; }
    public string? PortfolioWebsiteUrl { get; set; }
    public string? TwitterUrl { get; set; }

    // Navigation Properties
    public ICollection<Skill> Skills { get; set; } = new List<Skill>();
    public ICollection<FreelancerEducation> Educations { get; set; } = new List<FreelancerEducation>();
    public ICollection<FreelancerCertification> Certifications { get; set; } = new List<FreelancerCertification>();
    public ICollection<FreelancerPortfolio> Portfolios { get; set; } = new List<FreelancerPortfolio>();
    public ICollection<FreelancerLanguage> Languages { get; set; } = new List<FreelancerLanguage>();
}
