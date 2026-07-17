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

    public int ProfileCompletionPercentage { get; set; }
    public ICollection<string> MissingSections { get; set; } = new List<string>();

    public void CalculateCompletion()
    {
        var missing = new List<string>();
        int pct = 0;

        // 1. Basic Info (20%): Email, PhoneNumber, Country, City, Address
        if (!string.IsNullOrEmpty(Email) &&
            !string.IsNullOrEmpty(PhoneNumber) &&
            !string.IsNullOrEmpty(Country) &&
            !string.IsNullOrEmpty(City) &&
            !string.IsNullOrEmpty(Address))
        {
            pct += 20;
        }
        else
        {
            missing.Add("Basic Info");
        }

        // 2. Professional Info (20%): ProfessionalTitle, AboutMe, PrimaryTechnologyStack, Specialization, HourlyRate
        if (!string.IsNullOrEmpty(ProfessionalTitle) &&
            !string.IsNullOrEmpty(AboutMe) &&
            !string.IsNullOrEmpty(PrimaryTechnologyStack) &&
            !string.IsNullOrEmpty(Specialization) &&
            HourlyRate.HasValue)
        {
            pct += 20;
        }
        else
        {
            missing.Add("Professional Info");
        }

        // 3. Skills (15%)
        if (Skills != null && Skills.Any())
        {
            pct += 15;
        }
        else
        {
            missing.Add("Skills");
        }

        // 4. Education (15%)
        if (Educations != null && Educations.Any())
        {
            pct += 15;
        }
        else
        {
            missing.Add("Education");
        }

        // 5. Experience (10%)
        if (!IsExperienced || (IsExperienced && (ExperienceYears.GetValueOrDefault() > 0 || ExperienceMonths.GetValueOrDefault() > 0)))
        {
            pct += 10;
        }
        else
        {
            missing.Add("Experience");
        }

        // 6. Portfolio (10%)
        if (Portfolios != null && Portfolios.Any())
        {
            pct += 10;
        }
        else
        {
            missing.Add("Portfolio");
        }

        // 7. Resume (10%)
        if (!string.IsNullOrEmpty(ResumeUrl))
        {
            pct += 10;
        }
        else
        {
            missing.Add("Resume");
        }

        ProfileCompletionPercentage = pct;
        MissingSections = missing;
    }
}
