using System;
using System.ComponentModel.DataAnnotations;
using Worklance.Domain.Enums;

namespace Worklance.Application.DTOs.AdminUserDTO
{
    public class UpdateVerificationStatusDto
    {
        [Required]
        public VerificationStatus Status { get; set; }

        public string? Reason { get; set; } 
    }
}
