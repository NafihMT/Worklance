using System.Collections.Generic;
using Worklance.Domain.Common;
using Worklance.Domain.Entities.Job;

namespace Worklance.Domain.Entities;

public class Skill : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public ICollection<CategorySkill> CategorySkills { get; set; } = new List<CategorySkill>();
    public ICollection<JobSkill> JobSkills { get; set; } = new List<JobSkill>();

    public ICollection<FreelancerProfile> FreelancerProfiles { get; set; } = new List<FreelancerProfile>();
}
