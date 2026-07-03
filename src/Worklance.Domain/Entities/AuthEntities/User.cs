using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Worklance.Domain.Enums.AuthEnums;


namespace Worklance.Domain.Entities.AuthEntities
{
    public class User
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string AadhaarNumber { get; set; } = string.Empty;
        public string AadhaarProofPath { get; set; } = string.Empty;
        public AccountType AccountType { get; set; }
        public UserRole Role { get; set; } = UserRole.User;
        public bool EmailVerified { get; set; } = false;
        public UserStatus Status { get; set; } = UserStatus.Pending;
        public AdminVerificationStatus AdminVerificationStatus { get; set; } = AdminVerificationStatus.Pending;
        public string? RejectionReason { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<EmailOtp> EmailOtps { get; set; } = new List<EmailOtp>();
        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

    }
}
