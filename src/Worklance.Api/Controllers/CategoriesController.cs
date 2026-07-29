using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Worklance.Application.Common.ApiResponse;
using Worklance.Application.DTOs.Category;
using Worklance.Application.Interfaces.Services;

namespace Worklance.Api.Controllers
{
    [ApiController]
    [Route("api/categories")]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _adminService;

        public CategoriesController(ICategoryService adminService)
        {
            _adminService = adminService;
        }

        [HttpPost]
        [Authorize(Roles = "2")]
        public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryRequest request)
        {
            var result = await _adminService.CreateCategoryAsync(request);
            return CreatedAtAction(
                nameof(GetCategoryById),
                new { id = result.Id },
                ApiResponse.Success(
                    result,
                    "Category created successfully",
                    StatusCodes.Status201Created));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "2")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] UpdateCategoryRequest request)
        {
            var result = await _adminService.UpdateCategoryAsync(id, request);
            return Ok(ApiResponse.Success(
                result,
                "Category updated successfully",
                StatusCodes.Status200OK));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "2")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            await _adminService.DeleteCategoryAsync(id);
            return Ok(ApiResponse.Success(
                "Category deleted successfully",
                StatusCodes.Status200OK));
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCategories()
        {
            var result = await _adminService.GetAllCategoriesAsync();
            return Ok(ApiResponse.Success(
                result,
                "Categories retrieved successfully",
                StatusCodes.Status200OK));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            var result = await _adminService.GetCategoryByIdAsync(id);
            return Ok(ApiResponse.Success(
                result,
                "Category retrieved successfully",
                StatusCodes.Status200OK));
        }

        [HttpPost("{categoryId}/skills")]
        [Authorize(Roles = "2")]
        public async Task<IActionResult> AddSkill(int categoryId, [FromBody] CreateSkillRequest request)
        {
            var result = await _adminService.AddSkillAsync(categoryId, request);
            return Ok(ApiResponse.Success(
                result,
                "Skill added successfully",
                StatusCodes.Status200OK));
        }

        [HttpPut("skills/{skillId}")]
        [Authorize(Roles = "2")]
        public async Task<IActionResult> UpdateSkill(int skillId, [FromBody] UpdateSkillRequest request)
        {
            var result = await _adminService.UpdateSkillAsync(skillId, request);
            return Ok(ApiResponse.Success(
                result,
                "Skill updated successfully",
                StatusCodes.Status200OK));
        }

        [HttpDelete("skills/{skillId}")]
        [Authorize(Roles = "2")]
        public async Task<IActionResult> DeleteSkill(int skillId)
        {
            await _adminService.DeleteSkillAsync(skillId);
            return Ok(ApiResponse.Success(
                "Skill deleted successfully",
                StatusCodes.Status200OK));
        }
    }
}
