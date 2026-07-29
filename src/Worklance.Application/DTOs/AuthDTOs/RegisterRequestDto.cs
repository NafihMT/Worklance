using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Worklance.Domain.Enums.AuthEnums;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("Worklance.Api")]
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("Worklance.Infrastructure")]
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("Worklance.Application.UnitTests")]
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("Worklance.Infrastructure.IntegrationTests")]

namespace Worklance.Application.DTOs.AuthDTOs
{
    public class RegisterRequestDto
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public long PhoneNumber { get; set; }
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
        public long AadhaarNumber { get; set; }
        public byte[] AadhaarImageBytes { get; set; }
        public AccountType AccountType { get; set; }
    }
}
