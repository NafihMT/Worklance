using System.Collections.Generic;
using Worklance.Domain.Common;
using Worklance.Domain.Entities.Job;

namespace Worklance.Domain.Entities;

public class Skill : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    // Navigation Property
    public ICollection<Category> Categories { get; set; } = new List<Category>();

    // Navigation Property
    public ICollection<FreelancerProfile> FreelancerProfiles { get; set; } = new List<FreelancerProfile>();
}
