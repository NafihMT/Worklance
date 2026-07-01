using System;
using System.Collections.Generic;
using Worklance.Domain.Enums;

namespace Worklance.Application.DTOs.FreelancerProfiles;

public class CreateFreelancerProfileDto
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
    public AvailabilityStatus Availability { get; set; } = AvailabilityStatus.NotAvailable;
    public WorkPreferenceType WorkPreference { get; set; } = WorkPreferenceType.Both;
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

public class CreateEducationDto
{
    public string Institution { get; set; } = string.Empty;
    public string Degree { get; set; } = string.Empty;
    public string FieldOfStudy { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsCurrent { get; set; }
    public string? Description { get; set; }
}

public class CreateCertificationDto
{
    public string Name { get; set; } = string.Empty;
    public string IssuingOrganization { get; set; } = string.Empty;
    public DateTime IssueDate { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public string? CredentialId { get; set; }
    public string? CredentialUrl { get; set; }
}

public class CreatePortfolioDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? ProjectUrl { get; set; }
    public string? ImageUrl { get; set; }
    public string? TechnologiesUsed { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}

public class CreateLanguageDto
{
    public string LanguageName { get; set; } = string.Empty;
    public LanguageProficiency Proficiency { get; set; } = LanguageProficiency.Basic;
}
