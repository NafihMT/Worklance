using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using Worklance.Application.DTOs.AuthDTOs;

namespace Worklance.Application.Validators.AuthValidation
{
    public class RegisterRequestValidator:AbstractValidator<RegisterRequestDto>
    {
        public RegisterRequestValidator()
        {
            RuleFor(x => x.FullName)
           .NotEmpty().WithMessage("Full Name is required.")
           .MinimumLength(3).WithMessage("Enter a valid Full Name")
           .Matches(@"^[a-zA-Z\s]+$").WithMessage("Full name cannot contain special characters or numbers. Please enter a valid full name.");

            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress();
            RuleFor(x => x.PhoneNumber)
                .NotEmpty()
                .Matches(@"^\d{10}$")
                .WithMessage("Phone number must contain exactly 10 digits.");
            
            RuleFor(x => x.Password)
                .NotEmpty()
                .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z\d]).{8,}$")
                .WithMessage("Password must contain at least 8 characters, including one uppercase letter, one lowercase letter, one number, and one special character.");
            
            RuleFor(x => x.ConfirmPassword)
                .NotEmpty()
                .Equal(x => x.Password)
                .WithMessage("Password and Confirm Password must match.");
            RuleFor(x => x.AadhaarNumber)
                .NotEmpty()
                .Length(12)
                .Matches(@"^\d{12}$");
            RuleFor(x => x.AadhaarImageBytes)
                .NotEmpty().WithMessage("Aadhaar Image is required");
            RuleFor(x => x.AccountType)
                .IsInEnum();
        }
    }
}
