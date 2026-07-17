using System.IO;
using System.Threading.Tasks;
using Worklance.Application.DTOs.FreelancerProfiles;

namespace Worklance.Application.Interfaces.Services;

public interface IFreelancerProfileService
{
    Task<FreelancerProfileDto> CreateProfileAsync(string userId, CreateFreelancerProfileDto dto);
    Task<FreelancerProfileDto> UpdateProfileAsync(string userId, UpdateFreelancerProfileDto dto);
    Task<FreelancerProfileDto?> GetProfileByUserIdAsync(string userId);
    Task<FreelancerProfileDto?> GetProfileByIdAsync(int id);
    Task<string> UploadResumeAsync(string userId, Stream fileStream, string fileName, long fileSize);
    Task DeleteResumeAsync(string userId);
    Task<(Stream FileStream, string ContentType, string FileName)> DownloadResumeAsync(string userId);
}
