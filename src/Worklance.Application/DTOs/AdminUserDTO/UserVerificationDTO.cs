using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Worklance.Application.DTOs.AdminUserDTO
{
    public class UserVerificationDTO
    {
        public string UserId { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string AccountType { get; set; } = string.Empty; 

        public string? ProfilePhotoUrl { get; set; }
        public string? Profession { get; set; }
        public string? Headline { get; set; }
        public string? Bio { get; set; }
        public string? Location { get; set; }
       
        public string? GitHubUrl { get; set; }
        public string? BehanceUrl { get; set; }
        public string? PortfolioUrl { get; set; }
        public string? BusinessName { get; set; }
        public string? Services { get; set; }

        public string? LinkedInUrl { get; set; }
        public string AadhaarNumber { get; set; } = string.Empty;
        public string AadhaarFrontImageUrl { get; set; } = string.Empty;
        public string AadhaarBackImageUrl { get; set; } = string.Empty;

        public string VerificationStatus { get; set; } = string.Empty;
    }
}
