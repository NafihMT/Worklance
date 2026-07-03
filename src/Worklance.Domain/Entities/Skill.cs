using System.Collections.Generic;
using Worklance.Domain.Common;

namespace Worklance.Domain.Entities;

public class Skill : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    // Navigation Property
    public ICollection<FreelancerProfile> FreelancerProfiles { get; set; } = new List<FreelancerProfile>();
}
