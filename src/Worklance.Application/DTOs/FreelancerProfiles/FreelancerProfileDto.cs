using System;
using System.Collections.Generic;
using Worklance.Domain.Enums;

namespace Worklance.Application.DTOs.FreelancerProfiles;

public class FreelancerProfileDto
{
    public int Id { get; set; }
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
    public AvailabilityStatus Availability { get; set; }
    public WorkPreferenceType WorkPreference { get; set; }
    public decimal? HourlyRate { get; set; }

    // Documents & Links
    public string? ResumeUrl { get; set; }
    public string? GitHubUrl { get; set; }
    public string? LinkedInUrl { get; set; }
    public string? PortfolioWebsiteUrl { get; set; }
    public string? TwitterUrl { get; set; }

    public ICollection<SkillDto> Skills { get; set; } = new List<SkillDto>();
    public ICollection<FreelancerEducationDto> Educations { get; set; } = new List<FreelancerEducationDto>();
    public ICollection<FreelancerCertificationDto> Certifications { get; set; } = new List<FreelancerCertificationDto>();
    public ICollection<FreelancerPortfolioDto> Portfolios { get; set; } = new List<FreelancerPortfolioDto>();
    public ICollection<FreelancerLanguageDto> Languages { get; set; } = new List<FreelancerLanguageDto>();
}
