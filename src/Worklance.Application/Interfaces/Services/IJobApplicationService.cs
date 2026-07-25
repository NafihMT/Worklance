using System.Collections.Generic;
using System.Threading.Tasks;
using Worklance.Application.DTOs.JobApplications;

namespace Worklance.Application.Interfaces.Services;

public interface IJobApplicationService
{
    Task<JobApplicationResponse> ApplyForJobAsync(int jobId, string userId, ApplyJobRequest request);
    Task<IEnumerable<JobApplicationResponse>> GetMyApplicationsAsync(string userId);
    Task<IEnumerable<JobApplicationResponse>> GetApplicationsForJobAsync(int jobId, string userId);
    Task<(byte[] FileBytes, string ContentType, string FileName)> DownloadCoverLetterAsync(int applicationId, string userId);
}
