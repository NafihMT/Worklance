using System.Collections.Generic;
using System.Threading.Tasks;
using Worklance.Application.DTOs.FreelancerProfiles;

namespace Worklance.Application.Interfaces.Queries;

public interface IFreelancerProfileQueryService
{
    Task<FreelancerProfileDto?> GetMyProfileAsync(string userId);
    Task<FreelancerProfileDto?> GetProfileByIdAsync(int id);
    Task<IReadOnlyList<FreelancerProfileDto>> SearchFreelancersAsync(string? searchTerm, string? skill, decimal? minHourlyRate, decimal? maxHourlyRate);
    Task<IReadOnlyList<SkillDto>> GetAllSkillsAsync();
}
