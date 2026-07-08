using System.Collections.Generic;
using Worklance.Domain.Common;
using Worklance.Domain.Entities.Job;

namespace Worklance.Domain.Entities;

public class Skill : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public int? CategoryId { get; set; }
    public Category? Category { get; set; }

    // Navigation Property
    public ICollection<FreelancerProfile> FreelancerProfiles { get; set; } = new List<FreelancerProfile>();
}
