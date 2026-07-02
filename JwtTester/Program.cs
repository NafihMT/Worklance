using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

class Program
{
    static void Main()
    {
        var secretKey = "WorklanceDevelopment2026July02_SecureKey";
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, "3"),
            new Claim(JwtRegisteredClaimNames.Email, "adminworklance@yopmail.com"),
            new Claim("FullName", "Super Admin"),
            new Claim("AccountType", "JobSeeker"),
            new Claim("Status", "Pending"),
            new Claim("role", "2")
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: "Worklance",
            audience: "WorklanceUsers",
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(60),
            signingCredentials: credentials);

        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenString = tokenHandler.WriteToken(token);
        Console.WriteLine(tokenString);

        // Now try to validate it
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "Worklance",
            ValidAudience = "WorklanceUsers",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
            RoleClaimType = ClaimTypes.Role
        };

        try
        {
            var principal = tokenHandler.ValidateToken(tokenString, validationParameters, out var validatedToken);
            Console.WriteLine("Token is valid!");
            Console.WriteLine("Is in role '2'? " + principal.IsInRole("2"));
            
            foreach (var claim in principal.Claims)
            {
                Console.WriteLine($"{claim.Type}: {claim.Value}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Validation Failed: " + ex.Message);
        }
    }
}
