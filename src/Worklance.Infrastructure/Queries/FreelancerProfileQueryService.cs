using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Worklance.Application.DTOs.FreelancerProfiles;
using Worklance.Application.Interfaces.Queries;
using Worklance.Infrastructure.Queries;

namespace Worklance.Infrastructure.Queries;

public class FreelancerProfileQueryService : IFreelancerProfileQueryService
{
    private readonly DapperContext _dapperContext;

    public FreelancerProfileQueryService(DapperContext dapperContext)
    {
        _dapperContext = dapperContext;
    }

    public async Task<FreelancerProfileDto?> GetMyProfileAsync(string userId)
    {
        using var connection = _dapperContext.CreateConnection();
        
        // First get the profile ID
        const string getProfileIdQuery = "SELECT Id FROM FreelancerProfiles WHERE UserId = @UserId";
        var profileId = await connection.QueryFirstOrDefaultAsync<int?>(getProfileIdQuery, new { UserId = userId });

        // Fetch User basic info from the Users table
        const string getUserQuery = "SELECT Id, FullName, Email, PhoneNumber FROM Users WHERE Id = @UserId";
        var user = await connection.QueryFirstOrDefaultAsync<dynamic>(getUserQuery, new { UserId = userId });

        if (user == null)
        {
            return null;
        }

        if (profileId != null)
        {
            var profile = await GetProfileByIdAsync(profileId.Value);
            if (profile != null)
            {
                profile.Email = user.Email ?? string.Empty;
                if (string.IsNullOrEmpty(profile.PhoneNumber))
                {
                    profile.PhoneNumber = user.PhoneNumber;
                }
                profile.CalculateCompletion();
                return profile;
            }
        }

        string fullName = user.FullName ?? string.Empty;
        var nameParts = fullName.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        var fName = nameParts.Length > 0 ? nameParts[0] : string.Empty;
        var lName = nameParts.Length > 1 ? string.Join(" ", nameParts.Skip(1)) : string.Empty;

        var defaultProfile = new FreelancerProfileDto
        {
            Id = 0,
            UserId = userId,
            FirstName = fName,
            LastName = lName,
            Username = string.Empty,
            Email = user.Email ?? string.Empty,
            PhoneNumber = user.PhoneNumber,
            DateOfBirth = null,
            Gender = null,
            ProfilePhotoUrl = null,
            Country = null,
            State = null,
            City = null,
            Address = null,
            ProfessionalTitle = string.Empty,
            AboutMe = string.Empty,
            PrimaryTechnologyStack = string.Empty,
            Specialization = string.Empty,
            IsExperienced = false,
            ExperienceYears = null,
            ExperienceMonths = null,
            Availability = default,
            WorkPreference = default,
            HourlyRate = null,
            ResumeUrl = null,
            GitHubUrl = null,
            LinkedInUrl = null,
            PortfolioWebsiteUrl = null,
            TwitterUrl = null,
            Skills = new List<SkillDto>(),
            Educations = new List<FreelancerEducationDto>(),
            Certifications = new List<FreelancerCertificationDto>(),
            Portfolios = new List<FreelancerPortfolioDto>(),
            Languages = new List<FreelancerLanguageDto>()
        };
        defaultProfile.CalculateCompletion();
        return defaultProfile;
    }

    public async Task<FreelancerProfileDto?> GetProfileByIdAsync(int id)
    {
        using var connection = _dapperContext.CreateConnection();

        const string query = @"
            SELECT * FROM FreelancerProfiles WHERE Id = @Id;
            SELECT s.* FROM Skills s INNER JOIN FreelancerSkills fs ON s.Id = fs.SkillId WHERE fs.FreelancerProfileId = @Id;
            SELECT * FROM FreelancerEducations WHERE FreelancerProfileId = @Id;
            SELECT * FROM FreelancerCertifications WHERE FreelancerProfileId = @Id;
            SELECT * FROM FreelancerPortfolios WHERE FreelancerProfileId = @Id;
            SELECT * FROM FreelancerLanguages WHERE FreelancerProfileId = @Id;
        ";

        using var multi = await connection.QueryMultipleAsync(query, new { Id = id });

        var profile = await multi.ReadFirstOrDefaultAsync<FreelancerProfileDto>();
        if (profile == null) return null;

        profile.Skills = (await multi.ReadAsync<SkillDto>()).ToList();
        profile.Educations = (await multi.ReadAsync<FreelancerEducationDto>()).ToList();
        profile.Certifications = (await multi.ReadAsync<FreelancerCertificationDto>()).ToList();
        profile.Portfolios = (await multi.ReadAsync<FreelancerPortfolioDto>()).ToList();
        profile.Languages = (await multi.ReadAsync<FreelancerLanguageDto>()).ToList();

        const string userEmailQuery = "SELECT Email FROM Users WHERE Id = @UserId";
        var userEmail = await connection.QueryFirstOrDefaultAsync<string>(userEmailQuery, new { UserId = profile.UserId });
        profile.Email = userEmail ?? string.Empty;

        profile.CalculateCompletion();
        return profile;
    }

    public async Task<IReadOnlyList<FreelancerProfileDto>> SearchFreelancersAsync(string? searchTerm, string? skill, decimal? minHourlyRate, decimal? maxHourlyRate)
    {
        using var connection = _dapperContext.CreateConnection();

        // Base search query
        var sql = @"
            SELECT p.* 
            FROM FreelancerProfiles p
            WHERE 1=1
        ";
        
        var parameters = new DynamicParameters();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            sql += " AND (p.FirstName LIKE @SearchTerm OR p.LastName LIKE @SearchTerm OR p.ProfessionalTitle LIKE @SearchTerm OR p.AboutMe LIKE @SearchTerm OR p.Specialization LIKE @SearchTerm)";
            parameters.Add("SearchTerm", $"%{searchTerm}%");
        }

        if (!string.IsNullOrWhiteSpace(skill))
        {
            sql += " AND p.Id IN (SELECT fs.FreelancerProfileId FROM FreelancerSkills fs INNER JOIN Skills s ON fs.SkillId = s.Id WHERE s.Name LIKE @Skill)";
            parameters.Add("Skill", $"%{skill}%");
        }

        if (minHourlyRate.HasValue)
        {
            sql += " AND p.HourlyRate >= @MinHourlyRate";
            parameters.Add("MinHourlyRate", minHourlyRate.Value);
        }

        if (maxHourlyRate.HasValue)
        {
            sql += " AND p.HourlyRate <= @MaxHourlyRate";
            parameters.Add("MaxHourlyRate", maxHourlyRate.Value);
        }

        var profiles = await connection.QueryAsync<FreelancerProfileDto>(sql, parameters);
        var profileList = profiles.ToList();

        // Populate skills for the searched profiles
        foreach (var profile in profileList)
        {
            const string skillQuery = @"
                SELECT s.* 
                FROM Skills s 
                INNER JOIN FreelancerSkills fs ON s.Id = fs.SkillId 
                WHERE fs.FreelancerProfileId = @ProfileId";
            var skills = await connection.QueryAsync<SkillDto>(skillQuery, new { ProfileId = profile.Id });
            profile.Skills = skills.ToList();
            const string userEmailQuery = "SELECT Email FROM Users WHERE Id = @UserId";
            var userEmail = await connection.QueryFirstOrDefaultAsync<string>(userEmailQuery, new { UserId = profile.UserId });
            profile.Email = userEmail ?? string.Empty;

            profile.CalculateCompletion();
        }

        return profileList;
    }

    public async Task<IReadOnlyList<SkillDto>> GetAllSkillsAsync()
    {
        using var connection = _dapperContext.CreateConnection();
        const string query = "SELECT * FROM Skills ORDER BY Name";
        var skills = await connection.QueryAsync<SkillDto>(query);
        return skills.ToList();
    }

    private void PopulateCompletionDetails(FreelancerProfileDto dto)
    {
        var missing = new List<string>();
        int pct = 0;

        // 1. Basic Info (20%): Email, PhoneNumber, Country, City, Address
        if (!string.IsNullOrEmpty(dto.Email) &&
            !string.IsNullOrEmpty(dto.PhoneNumber) &&
            !string.IsNullOrEmpty(dto.Country) &&
            !string.IsNullOrEmpty(dto.City) &&
            !string.IsNullOrEmpty(dto.Address))
        {
            pct += 20;
        }
        else
        {
            missing.Add("Basic Info");
        }

        // 2. Professional Info (20%): ProfessionalTitle, AboutMe, PrimaryTechnologyStack, Specialization, HourlyRate
        if (!string.IsNullOrEmpty(dto.ProfessionalTitle) &&
            !string.IsNullOrEmpty(dto.AboutMe) &&
            !string.IsNullOrEmpty(dto.PrimaryTechnologyStack) &&
            !string.IsNullOrEmpty(dto.Specialization) &&
            dto.HourlyRate.HasValue)
        {
            pct += 20;
        }
        else
        {
            missing.Add("Professional Info");
        }

        // 3. Skills (15%)
        if (dto.Skills != null && dto.Skills.Any())
        {
            pct += 15;
        }
        else
        {
            missing.Add("Skills");
        }

        // 4. Education (15%)
        if (dto.Educations != null && dto.Educations.Any())
        {
            pct += 15;
        }
        else
        {
            missing.Add("Education");
        }

        // 5. Experience (10%)
        if (!dto.IsExperienced || (dto.IsExperienced && (dto.ExperienceYears.GetValueOrDefault() > 0 || dto.ExperienceMonths.GetValueOrDefault() > 0)))
        {
            pct += 10;
        }
        else
        {
            missing.Add("Experience");
        }

        // 6. Portfolio (10%)
        if (dto.Portfolios != null && dto.Portfolios.Any())
        {
            pct += 10;
        }
        else
        {
            missing.Add("Portfolio");
        }

        // 7. Resume (10%)
        if (!string.IsNullOrEmpty(dto.ResumeUrl))
        {
            pct += 10;
        }
        else
        {
            missing.Add("Resume");
        }

        dto.ProfileCompletionPercentage = pct;
        dto.MissingSections = missing;
    }
}
