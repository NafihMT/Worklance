using Worklance.Domain.Common;
using Worklance.Domain.Enums;

namespace Worklance.Domain.Entities;

public class FreelancerLanguage : BaseEntity
{
    public int FreelancerProfileId { get; set; }
    public FreelancerProfile FreelancerProfile { get; set; } = null!;
    
    public string LanguageName { get; set; } = string.Empty;
    public LanguageProficiency Proficiency { get; set; } = LanguageProficiency.Basic;
}
