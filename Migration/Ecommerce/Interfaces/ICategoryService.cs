using Ecommerce.Models;
using Ecommerce.Models.DTOs;

namespace Ecommerce.Interfaces
{
    public interface ICategoryService
    {
        Task<CategoryResponseDto> CreateCategory(CategoryRequestDto newCategory);
        Task<IEnumerable<Category>> GetAllCategories();
        Task<CategoryResponseDto> GetCategoryById(int id);
        Task<CategoryResponseDto> UpdateCategory(int id, CategoryRequestDto updateDto);
        Task<CategoryResponseDto> DeleteCategory(int id);
    }
}