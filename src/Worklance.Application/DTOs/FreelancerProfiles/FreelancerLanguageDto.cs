using Worklance.Domain.Enums;

namespace Worklance.Application.DTOs.FreelancerProfiles;

public class FreelancerLanguageDto
{
    public int Id { get; set; }
    public string LanguageName { get; set; } = string.Empty;
    public LanguageProficiency Proficiency { get; set; }
}
