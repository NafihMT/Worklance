using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Worklance.Application.DTOs.Category;
using Worklance.Application.Exceptions;
using Worklance.Application.Interfaces.Repositories;
using Worklance.Application.Interfaces.Services;
using Worklance.Domain.Entities;
using Worklance.Domain.Entities.Job;

namespace Worklance.Application.Services
{
    public class AdminService : IAdminService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly ISkillRepository _skillRepository;
        private readonly IUnitOfWork _unitOfWork;

        public AdminService(
            ICategoryRepository categoryRepository,
            ISkillRepository skillRepository,
            IUnitOfWork unitOfWork)
        {
            _categoryRepository = categoryRepository;
            _skillRepository = skillRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<CategoryResponse> CreateCategoryAsync(CreateCategoryRequest request)
        {
            if (await _categoryRepository.ExistsByNameAsync(request.Name))
                throw new BadRequestException("Category already exists.");

            var category = new Category
            {
                Name = request.Name.Trim(),
                Description = request.Description,
                CreatedAt = DateTime.UtcNow,
                LastModifiedAt = DateTime.UtcNow
            };

            if (request.Skills != null)
            {
                foreach (var skillName in request.Skills.Distinct(StringComparer.OrdinalIgnoreCase))
                {
                    var trimmedSkillName = skillName.Trim();
                    if (string.IsNullOrEmpty(trimmedSkillName)) continue;

                    // Validate duplicate skill name globally
                    var existingSkill = await _skillRepository.GetByNameAsync(trimmedSkillName);
                    if (existingSkill != null)
                        throw new BadRequestException($"Skill '{trimmedSkillName}' already exists.");

                    category.Skills.Add(new Skill
                    {
                        Name = trimmedSkillName
                    });
                }
            }

            await _categoryRepository.AddAsync(category);
            await _unitOfWork.SaveChangesAsync();

            return MapToResponse(category);
        }

        public async Task<CategoryResponse> UpdateCategoryAsync(int categoryId, UpdateCategoryRequest request)
        {
            var category = await _categoryRepository.GetByIdWithSkillsAsync(categoryId);
            if (category == null)
                throw new NotFoundException("Category not found.");

            if (await _categoryRepository.ExistsByNameExcludingIdAsync(request.Name, categoryId))
                throw new BadRequestException("Category with this name already exists.");

            category.Name = request.Name.Trim();
            category.Description = request.Description;
            category.LastModifiedAt = DateTime.UtcNow;

            _categoryRepository.Update(category);
            await _unitOfWork.SaveChangesAsync();

            return MapToResponse(category);
        }

        public async Task DeleteCategoryAsync(int categoryId)
        {
            var category = await _categoryRepository.GetByIdWithSkillsAsync(categoryId);
            if (category == null)
                throw new NotFoundException("Category not found.");

            if (category.IsDeleted)
                throw new BadRequestException("Category is already deleted.");

            category.IsDeleted = true;
            category.LastModifiedAt = DateTime.UtcNow;

            _categoryRepository.Update(category);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<CategoryResponse> AddSkillAsync(int categoryId, CreateSkillRequest request)
        {
            var category = await _categoryRepository.GetByIdWithSkillsAsync(categoryId);
            if (category == null)
                throw new NotFoundException("Category not found.");

            var trimmedSkillName = request.Name.Trim();
            var existingSkill = await _skillRepository.GetByNameAsync(trimmedSkillName);
            if (existingSkill != null)
                throw new BadRequestException($"Skill '{trimmedSkillName}' already exists.");

            var skill = new Skill
            {
                Name = trimmedSkillName,
                CategoryId = categoryId
            };

            await _skillRepository.AddAsync(skill);
            await _unitOfWork.SaveChangesAsync();

            // Refresh category to include the new skill
            var updatedCategory = await _categoryRepository.GetByIdWithSkillsAsync(categoryId);
            return MapToResponse(updatedCategory ?? category);
        }

        public async Task<CategoryResponse> UpdateSkillAsync(int skillId, UpdateSkillRequest request)
        {
            var skill = await _skillRepository.GetByIdAsync(skillId);
            if (skill == null)
                throw new NotFoundException("Skill not found.");

            var trimmedSkillName = request.Name.Trim();
            var existingSkill = await _skillRepository.GetByNameAsync(trimmedSkillName);
            if (existingSkill != null && existingSkill.Id != skillId)
                throw new BadRequestException($"Skill '{trimmedSkillName}' already exists.");

            skill.Name = trimmedSkillName;
            _skillRepository.Update(skill);
            await _unitOfWork.SaveChangesAsync();

            if (skill.CategoryId.HasValue)
            {
                var category = await _categoryRepository.GetByIdWithSkillsAsync(skill.CategoryId.Value);
                if (category != null)
                {
                    return MapToResponse(category);
                }
            }

            return new CategoryResponse
            {
                Id = 0,
                Name = "",
                Description = "",
                Skills = new List<string> { skill.Name }
            };
        }

        public async Task DeleteSkillAsync(int skillId)
        {
            var skill = await _skillRepository.GetByIdAsync(skillId);
            if (skill == null)
                throw new NotFoundException("Skill not found.");

            _skillRepository.Delete(skill);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<IEnumerable<CategoryResponse>> GetAllCategoriesAsync()
        {
            var categories = await _categoryRepository.GetAllCategoriesWithSkillsAsync();
            return categories.Select(MapToResponse);
        }

        public async Task<CategoryResponse> GetCategoryByIdAsync(int id)
        {
            var category = await _categoryRepository.GetByIdWithSkillsAsync(id);
            if (category == null)
                throw new NotFoundException("Category not found.");

            return MapToResponse(category);
        }

        private static CategoryResponse MapToResponse(Category category)
        {
            return new CategoryResponse
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                Skills = category.Skills?.Select(s => s.Name).ToList() ?? new List<string>()
            };
        }
    }
}
