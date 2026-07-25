using Worklance.Domain.Common;

namespace Worklance.Domain.Entities.Job
{
    public class Category : BaseAuditableEntity
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsDeleted { get; set; } = false;

        public ICollection<CategorySkill> CategorySkills { get; set; } = new List<CategorySkill>();
    }

    public class CategorySkill
    {
        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;
        public int SkillId { get; set; }
        public Skill Skill { get; set; } = null!;
    }
}
