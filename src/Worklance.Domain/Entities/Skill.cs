using System.Collections.Generic;
using Worklance.Domain.Common;
using Worklance.Domain.Entities.Job;

namespace Worklance.Domain.Entities;

public class Skill : BaseAuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public bool IsDeleted { get; set; } = false;

    public ICollection<CategorySkill> CategorySkills { get; set; } = new List<CategorySkill>();
    public ICollection<JobSkill> JobSkills { get; set; } = new List<JobSkill>();

    public ICollection<FreelancerProfile> FreelancerProfiles { get; set; } = new List<FreelancerProfile>();
}
