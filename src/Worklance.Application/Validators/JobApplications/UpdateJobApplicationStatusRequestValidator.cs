using FluentValidation;
using Worklance.Application.DTOs.JobApplications;
using Worklance.Domain.Enums.Job;

namespace Worklance.Application.Validators.JobApplications;

public class UpdateJobApplicationStatusRequestValidator : AbstractValidator<UpdateJobApplicationStatusRequest>
{
    public UpdateJobApplicationStatusRequestValidator()
    {
        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("Invalid job application status.")
            .Must(status => status == JobApplicationStatus.Accepted || status == JobApplicationStatus.Rejected || status == JobApplicationStatus.Shortlisted)
            .WithMessage("Status must be set to Accepted, Rejected, or Shortlisted.");
    }
}
