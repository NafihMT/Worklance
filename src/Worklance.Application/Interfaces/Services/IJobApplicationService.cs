using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Worklance.Application.DTOs.FreelancerProfiles;
using Worklance.Application.DTOs.JobApplications;

namespace Worklance.Application.Interfaces.Services;

public interface IJobApplicationService
{
    Task<JobApplicationResponse> ApplyForJobAsync(int jobId, string userId, ApplyJobRequest request);
    Task<IEnumerable<JobApplicationResponse>> GetMyApplicationsAsync(string userId);
    Task<IEnumerable<JobApplicationResponse>> GetApplicationsForJobAsync(int jobId, string userId);
    Task<IEnumerable<FreelancerProfileDto>> GetApplicantProfilesForJobAsync(int jobId, string userId);
    Task<(Stream FileStream, string ContentType, string FileName)> DownloadCoverLetterAsync(int applicationId, string userId);
}
