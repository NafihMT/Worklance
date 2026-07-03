using System;
using System.Collections.Generic;
using Worklance.Domain.Enums;

namespace Worklance.Application.DTOs.FreelancerProfiles;

public class UpdateFreelancerProfileDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public Gender? Gender { get; set; }
    
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

    // Social Links
    public string? GitHubUrl { get; set; }
    public string? LinkedInUrl { get; set; }
    public string? PortfolioWebsiteUrl { get; set; }
    public string? TwitterUrl { get; set; }

    // Nested lists
    public List<string> Skills { get; set; } = new();
    public List<CreateEducationDto> Educations { get; set; } = new();
    public List<CreateCertificationDto> Certifications { get; set; } = new();
    public List<CreatePortfolioDto> Portfolios { get; set; } = new();
    public List<CreateLanguageDto> Languages { get; set; } = new();
}
