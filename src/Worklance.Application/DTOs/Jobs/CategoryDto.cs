
namespace Worklance.Application.DTOs.Jobs
{
    public class CategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public List<CategorySkillDto> Skills { get; set; } = new List<CategorySkillDto>();
    }

    public class CategorySkillDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
