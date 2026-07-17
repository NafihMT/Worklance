using Worklance.Domain.Common;

namespace Worklance.Domain.Entities.Job
{
    public class Category : BaseAuditableEntity
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsDeleted { get; set; } = false;

        public ICollection<Skill> Skills { get; set; } = new List<Skill>();
    }
}
