using Ecommerce.Interfaces;
using Ecommerce.Models;
using Ecommerce.Models.DTOs;
using RealEstateApi.Exceptions;

namespace Ecommerce.Services
{
    public class ProductService : IProductService
    {
        private readonly IRepository<int, Product> _productRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ProductService(IRepository<int, Product> productRepository, IHttpContextAccessor httpContextAccessor)
        {
            _productRepository = productRepository;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<ProductResponseDto> CreateProduct(ProductRequestDto product)
        {
            var basePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/products");

            var products = await _productRepository.GetAllAsync();
            var existing = products.SingleOrDefault(p => string.Equals(p.ProductName, product.ProductName, StringComparison.OrdinalIgnoreCase));
            if (existing != null)
                throw new Exception("Product already exists.");
            
            var originalFileName = Path.GetFileName(product.Image.FileName);
            var extension = Path.GetExtension(originalFileName);
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };

            if (!allowedExtensions.Contains(extension.ToLower()))
                throw new FailedOperationException("Unsupported image format.");

            if (!Directory.Exists(basePath))
                Directory.CreateDirectory(basePath);
            
            var uniqueName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(basePath, uniqueName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await product.Image.CopyToAsync(stream);
            }
                
            Product newProduct = new Product
            {
                ProductName = product.ProductName,
                // Image = product.Image,
                Image = $"/uploads/products/{uniqueName}",
                Price = product.Price,
                UserId = product.UserId,
                CategoryId = product.CategoryId,
                ColorId = product.ColorId,
                ModelId = product.ModelId,
                SellStartDate = product.SellStartDate,
                SellEndDate = product.SellEndDate,
                IsNew = product.IsNew
            };
            newProduct = await _productRepository.AddAsync(newProduct);

            var request = _httpContextAccessor.HttpContext?.Request;

            if (request == null)
                throw new NotFoundException("HTTP context not available.");

            var baseUrl = $"{request.Scheme}://{request.Host}";

            var imageUrl = $"{baseUrl}{newProduct.Image}";

            return new ProductResponseDto
            {
                ProductId = newProduct.ProductId,
                ProductName = newProduct.ProductName,
                // Image = newProduct.Image,
                Image = imageUrl,
                Price = newProduct.Price,
                UserId = newProduct.UserId,
                CategoryId = newProduct.CategoryId,
                ColorId = newProduct.ColorId,
                ModelId = newProduct.ModelId,
                SellStartDate = newProduct.SellStartDate,
                SellEndDate = newProduct.SellEndDate,
                IsNew = newProduct.IsNew
            };
        }

        public async Task<ProductResponseDto> DeleteProduct(int id)
        {
            var product = await _productRepository.DeleteAsync(id);
            return new ProductResponseDto
            { 
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                Image = product.Image,
                Price = product.Price,
                UserId = product.UserId,
                CategoryId = product.CategoryId,
                ColorId = product.ColorId,
                ModelId = product.ModelId,
                SellStartDate = product.SellStartDate,
                SellEndDate = product.SellEndDate,
                IsNew = product.IsNew
            };

        }

        public async Task<IEnumerable<ProductResponseDto>> GetAllProducts(ProductQueryParamsDto paramsDto)
        {
            var products = await _productRepository.GetAllAsync();
            if (!string.IsNullOrWhiteSpace(paramsDto.CategoryName))
                products = products.Where(p => p.Category.CategoryName == paramsDto.CategoryName).ToList();
            return products.Select(product =>
                {
                    var request = _httpContextAccessor.HttpContext?.Request;

                    if (request == null)
                        throw new NotFoundException("HTTP context not available.");

                    var baseUrl = $"{request.Scheme}://{request.Host}";

                    var imageUrl = $"{baseUrl}{product.Image}";
                    return new ProductResponseDto
                    {
                        ProductId = product.ProductId,
                        ProductName = product.ProductName,
                        Image = imageUrl,
                        Price = product.Price,
                        UserId = product.UserId,
                        CategoryId = product.CategoryId,
                        ColorId = product.ColorId,
                        ModelId = product.ModelId,
                        SellStartDate = product.SellStartDate,
                        SellEndDate = product.SellEndDate,
                        IsNew = product.IsNew
                    };
                });
        }

        public async Task<ProductResponseDto> GetProductById(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            var request = _httpContextAccessor.HttpContext?.Request;

            if (request == null)
                throw new NotFoundException("HTTP context not available.");

            var baseUrl = $"{request.Scheme}://{request.Host}";

            var imageUrl = $"{baseUrl}{product.Image}";
            return new ProductResponseDto
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                Image = imageUrl,
                Price = product.Price,
                UserId = product.UserId,
                CategoryId = product.CategoryId,
                ColorId = product.ColorId,
                ModelId = product.ModelId,
                SellStartDate = product.SellStartDate,
                SellEndDate = product.SellEndDate,
                IsNew = product.IsNew
            };
        }

        public async Task<ProductResponseDto> UpdateProduct(int id, ProductUpdateRequestDto updateDto)
        {
            var basePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/products");

            var product = await _productRepository.GetByIdAsync(id);
            product.ProductName = updateDto.ProductName;
            

            // product.Image = product.Image;
            if (updateDto.Image != null)
            {
                var originalFileName = Path.GetFileName(updateDto.Image.FileName);
                var extension = Path.GetExtension(originalFileName);
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };

                if (!allowedExtensions.Contains(extension.ToLower()))
                    throw new FailedOperationException("Unsupported image format.");

                var uniqueName = $"{Guid.NewGuid()}{extension}";
                var filePath = Path.Combine(basePath, uniqueName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await updateDto.Image.CopyToAsync(stream);
                }

                product.Image = $"/uploads/products/{uniqueName}";
            }
            product.Price = updateDto.Price;
            product.UserId = updateDto.UserId;
            product.CategoryId = updateDto.CategoryId;
            product.ColorId = updateDto.ColorId;
            product.ModelId = updateDto.ModelId;
            product.SellStartDate = updateDto.SellStartDate;
            product.SellEndDate = updateDto.SellEndDate;
            product.IsNew = updateDto.IsNew;
        
            var updatedProduct = await _productRepository.UpdateAsync(id, product);

            return new ProductResponseDto
            { 
                ProductId = updatedProduct.ProductId,
                ProductName = updatedProduct.ProductName,
                Image = updatedProduct.Image,
                Price = updatedProduct.Price,
                UserId = updatedProduct.UserId,
                CategoryId = updatedProduct.CategoryId,
                ColorId = updatedProduct.ColorId,
                ModelId = updatedProduct.ModelId,
                SellStartDate = updatedProduct.SellStartDate,
                SellEndDate = updatedProduct.SellEndDate,
                IsNew = updatedProduct.IsNew
            };
        }
        
    }
}