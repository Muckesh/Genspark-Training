using Ecommerce.Interfaces;
using Ecommerce.Models;
using Ecommerce.Models.DTOs;

namespace Ecommerce.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IRepository<int, Category> _categoryRepository;
        public CategoryService(IRepository<int, Category> categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }
        public async Task<CategoryResponseDto> CreateCategory(CategoryRequestDto newCategory)
        {
            var categories = await _categoryRepository.GetAllAsync();
            var existing = categories.SingleOrDefault(c => string.Equals(c.CategoryName, newCategory.CategoryName, StringComparison.OrdinalIgnoreCase));
            if (existing != null)
                throw new Exception("Category Already Exists.");
            Category category = new Category
            {
                CategoryName = newCategory.CategoryName
            };
            category = await _categoryRepository.AddAsync(category);
            return new CategoryResponseDto
            {
                CategoryId = category.CategoryId,
                CategoryName = category.CategoryName
            };
        }

        public async Task<IEnumerable<Category>> GetAllCategories()
        {
            var categories = await _categoryRepository.GetAllAsync();
            return categories;
        }

        public async Task<CategoryResponseDto> GetCategoryById(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            return new CategoryResponseDto
            {
                CategoryId = category.CategoryId,
                CategoryName = category.CategoryName
            };
        }

        public async Task<CategoryResponseDto> UpdateCategory(int id, CategoryRequestDto updateDto)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
            {
                throw new KeyNotFoundException("Category not found.");
            }
            // Category updateCategory = new Category
            // {
            //     CategoryId = id,
            //     CategoryName = category.CategoryName
            // };
            category.CategoryName = updateDto.CategoryName;
            category = await _categoryRepository.UpdateAsync(id, category);
            return new CategoryResponseDto
            {
                CategoryId = category.CategoryId,
                CategoryName = category.CategoryName
            };
        }
        
        public async Task<CategoryResponseDto> DeleteCategory(int id)
        {
            var category = await _categoryRepository.DeleteAsync(id);
            return new CategoryResponseDto
            {
                CategoryId = category.CategoryId,
                CategoryName = category.CategoryName
            };
        }
    }
}