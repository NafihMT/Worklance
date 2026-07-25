using Microsoft.AspNetCore.Http;

namespace Worklance.Application.DTOs.JobApplications;

public class ApplyJobRequest
{
    public IFormFile CoverLetterFile { get; set; } = null!;
    public decimal ProposedRate { get; set; }
    public int EstimatedDays { get; set; }
}
