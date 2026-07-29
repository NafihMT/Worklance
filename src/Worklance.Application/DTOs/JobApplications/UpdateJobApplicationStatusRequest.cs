using Worklance.Domain.Enums.Job;

namespace Worklance.Application.DTOs.JobApplications;

public class UpdateJobApplicationStatusRequest
{
    public JobApplicationStatus Status { get; set; }
}
