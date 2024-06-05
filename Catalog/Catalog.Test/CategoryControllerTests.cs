using Catalog.API.Controllers;
using Catalog.Core.DTO.Category;
using Catalog.Core.Entities;
using Catalog.Core.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace Catalog.Test;

public class CategoryControllerTests
{
    private readonly Mock<ICategoryRepository> _categoryRepositoryMock;
    private readonly Mock<ILogger<CategoryController>> _loggerMock;
    private readonly CategoryController _controller;

    public CategoryControllerTests()
    {
        _categoryRepositoryMock = new Mock<ICategoryRepository>();
        _loggerMock = new Mock<ILogger<CategoryController>>();

        _controller = new CategoryController(_categoryRepositoryMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async void Get_ReturnsOkWithCategories()
    {
        // Arrange
        var categories = new List<Category>
        {
            new Category { Id = 1, Name = "Category 1" },
            new Category { Id = 2, Name = "Category 2" }
        };

        _categoryRepositoryMock.Setup(repo => repo.GetAllAsync())
            .ReturnsAsync(categories);
        
        // Act
        var result = await _controller.Get();
        
        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<List<CategoryGetRes>>(okResult.Value);
        Assert.Equal(2, returnValue.Count);
    }

    [Fact]
    public async void Get_ReturnsNoContent()
    {
        // Arrange
        _categoryRepositoryMock.Setup(repo => repo.GetAllAsync())
            .ReturnsAsync(new List<Category>());
        
        // Act
        var result = await _controller.Get();
        
        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async void GetCategory_NonExistingId_ReturnsNotFount()
    {
        // Arrange
        _categoryRepositoryMock.Setup(repo => repo.GetByIdAsync(1))
            .ReturnsAsync(new Category());
        
        // Act
        var result = await _controller.GetCategoryById(1);
        
        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async void GetCategory_ExistingId_ReturnsOk()
    {
        // Arrange
        var category = new Category { Id = 1, Name = "Category 1" };

        _categoryRepositoryMock.Setup(repo => repo.GetByIdAsync(1))
            .ReturnsAsync(category);
        
        // Act
        var result = await _controller.GetCategoryById(1);
        
        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<CategoryGetRes>(okResult.Value);
        Assert.Equal(category.Name, returnValue.Name);
    }

    [Fact]
    public async void Post_ValidCategory_ReturnsCreatedResult()
    {
        // Arrange
        var categoryDtoReq = new CategoryPostReq
        {
            Name = "Category 1"
        };

        var category = new Category
        {
            Id = 1,
            Name = categoryDtoReq.Name
        };

        _categoryRepositoryMock.Setup(repo => repo.CreateAsync(It.IsAny<Category>()))
            .Returns(Task.CompletedTask);
        
        // Act
        var result = await _controller.Post(categoryDtoReq);
        
        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        var returnValue = Assert.IsType<Category>(createdResult.Value);
        Assert.Equal(category.Name, returnValue.Name);
        _categoryRepositoryMock.Verify(repo => repo.CreateAsync(It.IsAny<Category>()), Times.Once);
    }

    [Fact]
    public async void Put_NonExistingId_ReturnsNotFound()
    {
        // Arrange
        var category = new Category();
        var categoryReq = new CategoryUpdateReq
        {
            Name = "Category"
        };
        
        _categoryRepositoryMock.Setup(repo => repo.GetByIdAsync(1))
            .ReturnsAsync(category);
        
        // Act
        var result = await _controller.Update(1, categoryReq);
        
        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async void Put_ValidId_ReturnsOk()
    {
        // Arrange
        var category = new Category { Id = 1, Name = "Category Tesst" };
        var categoryReq = new CategoryUpdateReq { Name = "Category Test" };

        _categoryRepositoryMock.Setup(repo => repo.GetByIdAsync(1))
            .ReturnsAsync(category);

        _categoryRepositoryMock.Setup(repo => repo.UpdateAsync(It.IsAny<Category>()))
            .Returns(Task.CompletedTask);
        
        // Act
        var result = await _controller.Update(1, categoryReq);
        
        // Assert
        Assert.IsType<OkResult>(result);
        _categoryRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Category>()), Times.Once);
    }

    [Fact]
    public async void Delete_NonExistingId_ReturnsNotFount()
    {
        // Arrange
        var category = new Category();
        var categoryReq = new CategoryUpdateReq
        {
            Name = "Category"
        };
        
        _categoryRepositoryMock.Setup(repo => repo.GetByIdAsync(1))
            .ReturnsAsync(category);
        
        // Act
        var result = await _controller.Delete(1);
        
        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async void Delete_ValidId_ReturnsOk()
    {
        // Arrange
        var category = new Category { Id = 1, Name = "Category Tesst" };

        _categoryRepositoryMock.Setup(repo => repo.GetByIdAsync(1))
            .ReturnsAsync(category);

        _categoryRepositoryMock.Setup(repo => repo.RemoveAsync(1))
            .Returns(Task.CompletedTask);
        
        // Act
        var result = await _controller.Delete(1);
        
        // Assert
        Assert.IsType<OkResult>(result);
        _categoryRepositoryMock.Verify(repo => repo.RemoveAsync(1), Times.Once);
    }
}