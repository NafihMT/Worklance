using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Worklance.Application.DTOs.AdminUserDTO
{
    public class UserVerificationDTO
    {
        public int UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string AccountType { get; set; } = string.Empty; 
        public string AadhaarNumber { get; set; } = string.Empty;
        public DateTime RegisteredAt { get; set; }

        public string OcrStatus { get; set; } = string.Empty;
    }
}
