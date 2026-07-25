using System.IO;
using System.Linq;
using FluentValidation;
using Worklance.Application.DTOs.JobApplications;

namespace Worklance.Application.Validators.JobApplications;

public class ApplyJobRequestValidator : AbstractValidator<ApplyJobRequest>
{
    private static readonly string[] AllowedExtensions = { ".pdf", ".doc", ".docx", ".txt" };
    private const long MaxFileSizeBytes = 5 * 1024 * 1024; // 5 MB

    public ApplyJobRequestValidator()
    {
        RuleFor(x => x.CoverLetterFile)
            .NotNull().WithMessage("Cover letter file is required.")
            .Must(file => file != null && file.Length > 0).WithMessage("Cover letter file cannot be empty.")
            .Must(file => file != null && file.Length <= MaxFileSizeBytes).WithMessage("Cover letter file size must not exceed 5 MB.")
            .Must(file =>
            {
                if (file == null) return false;
                var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
                return AllowedExtensions.Contains(ext);
            }).WithMessage("Cover letter file must be a document (.pdf, .doc, .docx, .txt).");

        RuleFor(x => x.ProposedRate)
            .GreaterThan(0).WithMessage("Proposed rate must be greater than zero.");

        RuleFor(x => x.EstimatedDays)
            .GreaterThan(0).WithMessage("Estimated completion days must be greater than zero.");
    }
}
