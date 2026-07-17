using System.ComponentModel.DataAnnotations;

namespace Worklance.Application.DTOs.Category;

public class UpdateCategoryRequest
{
    [Required]
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}
