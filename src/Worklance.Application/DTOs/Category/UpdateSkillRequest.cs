using System.ComponentModel.DataAnnotations;

namespace Worklance.Application.DTOs.Category;

public class UpdateSkillRequest
{
    [Required]
    public string Name { get; set; } = string.Empty;
}
