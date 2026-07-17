using System.ComponentModel.DataAnnotations;

namespace Worklance.Application.DTOs.Category;

public class CreateCategoryRequest
{
    [Required]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    [MinLength(1)]
    public List<string> Skills { get; set; } = new();
}