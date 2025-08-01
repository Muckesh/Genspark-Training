
using Ecommerce.Controllers;
using Ecommerce.Interfaces;
using Ecommerce.Models.DTOs;
using Ecommerce.Models;

using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Ecommerce.Tests.Controllers
{
    public class CategoriesControllerTests
    {
        private readonly Mock<ICategoryService> _categoryServiceMock;
        private readonly CategoriesController _controller;

        public CategoriesControllerTests()
        {
            _categoryServiceMock = new Mock<ICategoryService>();
            _controller = new CategoriesController(_categoryServiceMock.Object);
        }

        [Fact]
        public async Task GetAll_ShouldReturnOkWithCategories()
        {
            // Arrange
            var categories = new List<Category>
            {
                new Category { CategoryId = 1, CategoryName = "Electronics" },
                new Category { CategoryId = 2, CategoryName = "Books" }
            };

            _categoryServiceMock
                .Setup(s => s.GetAllCategories())
                .ReturnsAsync(categories);


            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<Category>>(okResult.Value);

            Assert.Equal(2, returnValue.Count());
        }

        [Fact]
        public async Task Get_ShouldReturnOkWithCategory()
        {
            // Arrange
            var category = new CategoryResponseDto { CategoryId = 1, CategoryName = "Electronics" };
            _categoryServiceMock.Setup(s => s.GetCategoryById(1)).ReturnsAsync(category);

            // Act
            var result = await _controller.Get(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<CategoryResponseDto>(okResult.Value);
            Assert.Equal("Electronics", returnValue.CategoryName);
        }

        [Fact]
        public async Task Create_ShouldReturnOkWithCreatedCategory()
        {
            // Arrange
            var createDto = new CategoryRequestDto { CategoryName = "Clothing" };
            var createdCategory = new CategoryResponseDto { CategoryId = 3, CategoryName = "Clothing" };

            _categoryServiceMock
                .Setup(s => s.CreateCategory(createDto))
                .ReturnsAsync(createdCategory);

            // Act
            var result = await _controller.Create(createDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<CategoryResponseDto>(okResult.Value);
            Assert.Equal("Clothing", returnValue.CategoryName);
        }

        [Fact]
        public async Task Update_ShouldReturnOkWithUpdatedCategory()
        {
            // Arrange
            var updateDto = new CategoryRequestDto { CategoryName = "Updated Name" };
            var existingCategory = new CategoryResponseDto { CategoryId = 1, CategoryName = "Old Name" };
            var updatedCategory = new CategoryResponseDto { CategoryId = 1, CategoryName = "Updated Name" };

            _categoryServiceMock.Setup(s => s.GetCategoryById(1)).ReturnsAsync(existingCategory);
            _categoryServiceMock.Setup(s => s.UpdateCategory(1, updateDto)).ReturnsAsync(updatedCategory);

            // Act
            var result = await _controller.Update(1, updateDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<CategoryResponseDto>(okResult.Value);
            Assert.Equal("Updated Name", returnValue.CategoryName);
        }

        [Fact]
        public async Task Delete_ShouldReturnOkWithDeletedCategory()
        {
            // Arrange
            var existingCategory = new CategoryResponseDto { CategoryId = 1, CategoryName = "To Delete" };
            var deletedCategory = new CategoryResponseDto { CategoryId = 1, CategoryName = "To Delete" };

            _categoryServiceMock.Setup(s => s.GetCategoryById(1)).ReturnsAsync(existingCategory);
            _categoryServiceMock.Setup(s => s.DeleteCategory(1)).ReturnsAsync(deletedCategory);

            // Act
            var result = await _controller.Delete(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<CategoryResponseDto>(okResult.Value);
            Assert.Equal(1, returnValue.CategoryId);
        }

        [Fact]
        public async Task Get_ShouldReturnBadRequest_WhenExceptionThrown()
        {
            // Arrange
            _categoryServiceMock.Setup(s => s.GetCategoryById(99)).ThrowsAsync(new Exception("Not Found"));

            // Act
            var result = await _controller.Get(99);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Not Found", badRequest.Value);
        }
    }
}
