using Worklance.Application.DTOs.Category;

namespace Worklance.Application.Interfaces.Services;

public interface ICategoryService
{
    Task<CategoryResponse> CreateCategoryAsync(CreateCategoryRequest request);
    Task<CategoryResponse> UpdateCategoryAsync(int categoryId, UpdateCategoryRequest request);
    Task DeleteCategoryAsync(int categoryId);
    Task<CategoryResponse> AddSkillAsync(int categoryId, CreateSkillRequest request);
    Task<CategoryResponse> UpdateSkillAsync(int skillId, UpdateSkillRequest request);
    Task DeleteSkillAsync(int skillId);
    Task<IEnumerable<CategoryResponse>> GetAllCategoriesAsync();
    Task<CategoryResponse> GetCategoryByIdAsync(int id);
}