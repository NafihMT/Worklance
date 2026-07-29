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

        if (profileId == null) return null;

        return await GetProfileByIdAsync(profileId.Value);
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
}
