using Ecommerce.Models.DTOs;

namespace Ecommerce.Interfaces
{
    public interface IProductService
    {
        Task<ProductResponseDto> CreateProduct(ProductRequestDto product);
        Task<IEnumerable<ProductResponseDto>> GetAllProducts(ProductQueryParamsDto paramsDto);
        Task<ProductResponseDto> GetProductById(int id);
        Task<ProductResponseDto> UpdateProduct(int id, ProductUpdateRequestDto updateDto);
        Task<ProductResponseDto> DeleteProduct(int id);
    }
}