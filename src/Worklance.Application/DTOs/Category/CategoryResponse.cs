namespace Worklance.Application.DTOs.Category;

public class CategoryResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public List<string> Skills { get; set; } = new();
}