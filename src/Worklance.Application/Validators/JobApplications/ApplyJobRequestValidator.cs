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
        RuleFor(x => x)
            .Must(x => !string.IsNullOrWhiteSpace(x.CoverLetterText) || x.CoverLetterFile != null)
            .WithMessage("Either a cover letter text or a cover letter document attachment must be provided.");

        When(x => !string.IsNullOrWhiteSpace(x.CoverLetterText), () =>
        {
            RuleFor(x => x.CoverLetterText)
                .MaximumLength(4000).WithMessage("Cover letter text must not exceed 4000 characters.");
        });

        When(x => x.CoverLetterFile != null, () =>
        {
            RuleFor(x => x.CoverLetterFile!)
                .Must(file => file.Length > 0).WithMessage("Uploaded cover letter file cannot be empty.")
                .Must(file => file.Length <= MaxFileSizeBytes).WithMessage("Cover letter file size must not exceed 5 MB.")
                .Must(file =>
                {
                    var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
                    return AllowedExtensions.Contains(ext);
                }).WithMessage("Cover letter file must be a valid document (.pdf, .doc, .docx, .txt).");
        });

        RuleFor(x => x.ProposedRate)
            .GreaterThan(0).WithMessage("Proposed rate must be greater than zero.");

        RuleFor(x => x.EstimatedDays)
            .GreaterThan(0).WithMessage("Estimated completion days must be greater than zero.");
    }
}
