using Microsoft.AspNetCore.Http;

namespace Worklance.Application.DTOs.JobApplications;

public class ApplyJobRequest
{
    public string? CoverLetterText { get; set; }
    public IFormFile? CoverLetterFile { get; set; }
    public decimal ProposedRate { get; set; }
    public int EstimatedDays { get; set; }
}
