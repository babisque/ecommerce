using Catalog.API.Controllers;
using Catalog.Core.DTO;
using Catalog.Core.Entities;
using Catalog.Core.Repositories;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace Catalog.Test;

public class ProductControllerTests
{
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly Mock<ILogger<ProductController>> _loggerMock;
    private readonly Mock<IValidator<Product>> _validatorMock;
    private readonly ProductController _controller;

    public ProductControllerTests()
    {
        _productRepositoryMock = new Mock<IProductRepository>();
        _loggerMock = new Mock<ILogger<ProductController>>();
        _validatorMock = new Mock<IValidator<Product>>();
        _controller = new ProductController(_productRepositoryMock.Object, _loggerMock.Object, _validatorMock.Object);
    }
    
    [Fact]
    public async Task Post_ValidProduct_ReturnsCreatedResult()
    {
        // Arrange
        var productDtoRequest = new ProductDtoRequest
        {
            Name = "Test Product",
            Description = "Test Description",
            Price = 100,
            Category = "Test Category",
            Stock = 10,
            Image = null
        };

        var product = new Product
        {
            Id = 1,
            Name = productDtoRequest.Name,
            Description = productDtoRequest.Description,
            Price = productDtoRequest.Price,
            Category = productDtoRequest.Category,
            Stock = productDtoRequest.Stock
        };

        _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<Product>(), default))
            .ReturnsAsync(new ValidationResult());

        _productRepositoryMock.Setup(repo => repo.CreateAsync(It.IsAny<Product>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Post(productDtoRequest);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var returnValue = Assert.IsType<Product>(createdResult.Value);
        Assert.Equal(product.Name, returnValue.Name);
        _productRepositoryMock.Verify(repo => repo.CreateAsync(It.IsAny<Product>()), Times.Once);
    }
    
    [Fact]
    public async Task Post_InvalidProduct_ReturnsBadRequest()
    {
        // Arrange
        var productDtoRequest = new ProductDtoRequest
        {
            Name = "Test Product",
            Description = "Test Description",
            Price = 100,
            Category = "Test Category",
            Stock = 10,
            Image = null
        };

        var validationFailures = new List<ValidationFailure>
        {
            new ValidationFailure("Name", "Name is required")
        };

        _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<Product>(), default))
            .ReturnsAsync(new ValidationResult(validationFailures));

        // Act
        var result = await _controller.Post(productDtoRequest);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        var problemDetails = Assert.IsType<ValidationProblemDetails>(badRequestResult.Value);
        Assert.Contains("Name", problemDetails.Errors);
    }
    
    [Fact]
    public async Task Get_ReturnsOkWithProducts()
    {
        // Arrange
        var products = new List<Product>
        {
            new Product { Id = 1, Name = "Product 1" },
            new Product { Id = 2, Name = "Product 2" }
        };

        _productRepositoryMock.Setup(repo => repo.GetAllAsync())
            .ReturnsAsync(products);

        // Act
        var result = await _controller.Get();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<List<ProductDtoResponse>>(okResult.Value);
        Assert.Equal(2, returnValue.Count);
    }
    
    [Fact]
    public async Task GetProduct_ExistingId_ReturnsOk()
    {
        // Arrange
        var product = new Product { Id = 1, Name = "Product 1" };

        _productRepositoryMock.Setup(repo => repo.GetByIdAsync(1))
            .ReturnsAsync(product);

        // Act
        var result = await _controller.GetProduct(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsType<ProductDtoResponse>(okResult.Value);
        Assert.Equal(product.Name, returnValue.Name);
    }
    
    [Fact]
    public async Task GetProduct_NonExistingId_ReturnsNotFound()
    {
        // Arrange
        _productRepositoryMock.Setup(repo => repo.GetByIdAsync(1))
            .ReturnsAsync((Product)null!);

        // Act
        var result = await _controller.GetProduct(1);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task Put_ValidUpdate_ReturnsOk()
    {
        // Arrange
        var product = new Product { Id = 1, Name = "Product 1", Price = 100 };
        var updateDto = new ProductDtoUpdate { Name = "Updated Product", Price = 150 };

        _productRepositoryMock.Setup(repo => repo.GetByIdAsync(1))
            .ReturnsAsync(product);

        _productRepositoryMock.Setup(repo => repo.UpdateAsync(It.IsAny<Product>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Put(1, updateDto);

        // Assert
        Assert.IsType<OkResult>(result);
        _productRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Product>()), Times.Once);
    }

    [Fact]
    public async Task Delete_ExistingId_ReturnsOk()
    {
        // Arrange
        var product = new Product { Id = 1, Name = "Product 1" };

        _productRepositoryMock.Setup(repo => repo.GetByIdAsync(1))
            .ReturnsAsync(product);

        _productRepositoryMock.Setup(repo => repo.RemoveAsync(1))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Delete(1);

        // Assert
        Assert.IsType<OkResult>(result);
        _productRepositoryMock.Verify(repo => repo.RemoveAsync(1), Times.Once);
    }

    [Fact]
    public async Task Delete_NonExistingId_ReturnsNotFound()
    {
        // Arrange
        _productRepositoryMock.Setup(repo => repo.GetByIdAsync(1))
            .ReturnsAsync((Product)null!);

        // Act
        var result = await _controller.Delete(1);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }
}