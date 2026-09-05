using API_Gestion_Inventario.DTOs.Products;
using API_Gestion_Inventario.Models;
using API_Gestion_Inventario.Repositories.Interfaces;
using API_Gestion_Inventario.Services;
using Moq;

namespace API_Gestion_Inventario.Tests.Services;

public class ProductServiceTests
{
    [Fact]
    public async Task GetAllAsync_ShouldReturnProducts()
    {
        // Arrange
        var products = new List<Product>
        {
            new()
            {
                Id = 1,
                Name = "Teclado Mecánico",
                Price = 180000,
                Stock = 15,
                MinimumStock = 5,
                CategoryId = 1,
                Category = new Category
                {
                    Id = 1,
                    Name = "Hogar y Oficina"
                }
            },
            new()
            {
                Id = 2,
                Name = "Mouse Inalámbrico",
                Price = 85000,
                Stock = 20,
                MinimumStock = 5,
                CategoryId = 1,
                Category = new Category
                {
                    Id = 1,
                    Name = "Hogar y Oficina"
                }
            }
        };

        var productRepository = new Mock<IProductRepository>();
        var categoryRepository = new Mock<ICategoryRepository>();

        productRepository
            .Setup(repository => repository.GetAllAsync())
            .ReturnsAsync(products);

        var service = new ProductService(
            productRepository.Object,
            categoryRepository.Object);

        // Act
        var result = (await service.GetAllAsync()).ToList();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Equal("Teclado Mecánico", result[0].Name);
        Assert.Equal("Mouse Inalámbrico", result[1].Name);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnProduct_WhenProductExists()
    {
        // Arrange
        var product = CreateProduct();

        var productRepository = new Mock<IProductRepository>();
        var categoryRepository = new Mock<ICategoryRepository>();

        productRepository
            .Setup(repository => repository.GetByIdAsync(1))
            .ReturnsAsync(product);

        var service = new ProductService(
            productRepository.Object,
            categoryRepository.Object);

        // Act
        var result = await service.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Teclado Mecánico", result.Name);
        Assert.Equal(180000, result.Price);
        Assert.Equal("Hogar y Oficina", result.CategoryName);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenProductDoesNotExist()
    {
        // Arrange
        var productRepository = new Mock<IProductRepository>();
        var categoryRepository = new Mock<ICategoryRepository>();

        productRepository
            .Setup(repository => repository.GetByIdAsync(99))
            .ReturnsAsync((Product?)null);

        var service = new ProductService(
            productRepository.Object,
            categoryRepository.Object);

        // Act
        var result = await service.GetByIdAsync(99);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateProduct_WhenDataIsValid()
    {
        // Arrange
        var request = new CreateProductRequest
        {
            Name = "Monitor",
            Description = "Monitor para oficina",
            Price = 700000,
            Stock = 10,
            MinimumStock = 3,
            CategoryId = 1
        };

        var category = new Category
        {
            Id = 1,
            Name = "Hogar y Oficina"
        };

        var productRepository = new Mock<IProductRepository>();
        var categoryRepository = new Mock<ICategoryRepository>();

        productRepository
            .Setup(repository => repository.GetByNameAsync(request.Name))
            .ReturnsAsync((Product?)null);

        categoryRepository
            .Setup(repository => repository.GetByIdAsync(request.CategoryId))
            .ReturnsAsync(category);

        productRepository
            .Setup(repository => repository.AddAsync(It.IsAny<Product>()))
            .Returns(Task.CompletedTask);

        var service = new ProductService(
            productRepository.Object,
            categoryRepository.Object);

        // Act
        var result = await service.CreateAsync(request);

        // Assert
        Assert.Equal("Monitor", result.Name);
        Assert.Equal(700000, result.Price);
        Assert.Equal(10, result.Stock);
        Assert.Equal(3, result.MinimumStock);
        Assert.Equal(1, result.CategoryId);
        Assert.Equal("Hogar y Oficina", result.CategoryName);

        productRepository.Verify(
            repository => repository.AddAsync(
                It.Is<Product>(p =>
                    p.Name == "Monitor" &&
                    p.Price == 700000 &&
                    p.Stock == 10 &&
                    p.MinimumStock == 3 &&
                    p.CategoryId == 1)),
            Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ShouldThrow_WhenProductNameAlreadyExists()
    {
        // Arrange
        var request = new CreateProductRequest
        {
            Name = "Teclado Mecánico",
            Price = 180000,
            Stock = 10,
            MinimumStock = 5,
            CategoryId = 1
        };

        var existingProduct = CreateProduct();

        var productRepository = new Mock<IProductRepository>();
        var categoryRepository = new Mock<ICategoryRepository>();

        productRepository
            .Setup(repository => repository.GetByNameAsync(request.Name))
            .ReturnsAsync(existingProduct);

        var service = new ProductService(
            productRepository.Object,
            categoryRepository.Object);

        // Act
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.CreateAsync(request));

        // Assert
        Assert.Equal(
            "Ya existe un producto con ese nombre.",
            exception.Message);

        productRepository.Verify(
            repository => repository.AddAsync(It.IsAny<Product>()),
            Times.Never);
    }

    [Fact]
    public async Task CreateAsync_ShouldThrow_WhenCategoryDoesNotExist()
    {
        // Arrange
        var request = new CreateProductRequest
        {
            Name = "Monitor",
            Price = 700000,
            Stock = 10,
            MinimumStock = 3,
            CategoryId = 99
        };

        var productRepository = new Mock<IProductRepository>();
        var categoryRepository = new Mock<ICategoryRepository>();

        productRepository
            .Setup(repository => repository.GetByNameAsync(request.Name))
            .ReturnsAsync((Product?)null);

        categoryRepository
            .Setup(repository => repository.GetByIdAsync(99))
            .ReturnsAsync((Category?)null);

        var service = new ProductService(
            productRepository.Object,
            categoryRepository.Object);

        // Act
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.CreateAsync(request));

        // Assert
        Assert.Equal(
            "La categoría especificada no existe.",
            exception.Message);

        productRepository.Verify(
            repository => repository.AddAsync(It.IsAny<Product>()),
            Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateProduct_WhenDataIsValid()
    {
        // Arrange
        var existingProduct = CreateProduct();

        var request = new UpdateProductRequest
        {
            Name = "Teclado Mecánico Pro",
            Description = "Teclado actualizado",
            Price = 250000,
            Stock = 20,
            MinimumStock = 5,
            CategoryId = 2
        };

        var category = new Category
        {
            Id = 2,
            Name = "Tecnología"
        };

        var productRepository = new Mock<IProductRepository>();
        var categoryRepository = new Mock<ICategoryRepository>();

        productRepository
            .Setup(repository => repository.GetByIdAsync(1))
            .ReturnsAsync(existingProduct);

        categoryRepository
            .Setup(repository => repository.GetByIdAsync(2))
            .ReturnsAsync(category);

        productRepository
            .Setup(repository => repository.GetByNameAsync(
                request.Name))
            .ReturnsAsync((Product?)null);

        productRepository
            .Setup(repository => repository.UpdateAsync(
                It.IsAny<Product>()))
            .Returns(Task.CompletedTask);

        var service = new ProductService(
            productRepository.Object,
            categoryRepository.Object);

        // Act
        var result = await service.UpdateAsync(1, request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Teclado Mecánico Pro", result.Name);
        Assert.Equal("Teclado actualizado", result.Description);
        Assert.Equal(250000, result.Price);
        Assert.Equal(20, result.Stock);
        Assert.Equal(5, result.MinimumStock);
        Assert.Equal(2, result.CategoryId);
        Assert.Equal("Tecnología", result.CategoryName);

        productRepository.Verify(
            repository => repository.UpdateAsync(
                It.Is<Product>(p =>
                    p.Id == 1 &&
                    p.Name == "Teclado Mecánico Pro" &&
                    p.Price == 250000 &&
                    p.Stock == 20 &&
                    p.CategoryId == 2)),
            Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnNull_WhenProductDoesNotExist()
    {
        // Arrange
        var request = new UpdateProductRequest
        {
            Name = "Producto",
            Price = 100000,
            Stock = 10,
            MinimumStock = 2,
            CategoryId = 1
        };

        var productRepository = new Mock<IProductRepository>();
        var categoryRepository = new Mock<ICategoryRepository>();

        productRepository
            .Setup(repository => repository.GetByIdAsync(99))
            .ReturnsAsync((Product?)null);

        var service = new ProductService(
            productRepository.Object,
            categoryRepository.Object);

        // Act
        var result = await service.UpdateAsync(99, request);

        // Assert
        Assert.Null(result);

        productRepository.Verify(
            repository => repository.UpdateAsync(It.IsAny<Product>()),
            Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrow_WhenCategoryDoesNotExist()
    {
        // Arrange
        var existingProduct = CreateProduct();

        var request = new UpdateProductRequest
        {
            Name = "Producto Actualizado",
            Price = 200000,
            Stock = 10,
            MinimumStock = 2,
            CategoryId = 99
        };

        var productRepository = new Mock<IProductRepository>();
        var categoryRepository = new Mock<ICategoryRepository>();

        productRepository
            .Setup(repository => repository.GetByIdAsync(1))
            .ReturnsAsync(existingProduct);

        categoryRepository
            .Setup(repository => repository.GetByIdAsync(99))
            .ReturnsAsync((Category?)null);

        var service = new ProductService(
            productRepository.Object,
            categoryRepository.Object);

        // Act
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.UpdateAsync(1, request));

        // Assert
        Assert.Equal(
            "La categoría especificada no existe.",
            exception.Message);

        productRepository.Verify(
            repository => repository.UpdateAsync(It.IsAny<Product>()),
            Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrow_WhenAnotherProductHasSameName()
    {
        // Arrange
        var existingProduct = CreateProduct();

        var anotherProduct = new Product
        {
            Id = 2,
            Name = "Mouse Inalámbrico",
            Price = 85000,
            Stock = 20,
            MinimumStock = 5,
            CategoryId = 1
        };

        var request = new UpdateProductRequest
        {
            Name = "Mouse Inalámbrico",
            Price = 200000,
            Stock = 10,
            MinimumStock = 2,
            CategoryId = 1
        };

        var category = new Category
        {
            Id = 1,
            Name = "Hogar y Oficina"
        };

        var productRepository = new Mock<IProductRepository>();
        var categoryRepository = new Mock<ICategoryRepository>();

        productRepository
            .Setup(repository => repository.GetByIdAsync(1))
            .ReturnsAsync(existingProduct);

        categoryRepository
            .Setup(repository => repository.GetByIdAsync(1))
            .ReturnsAsync(category);

        productRepository
            .Setup(repository => repository.GetByNameAsync(
                request.Name))
            .ReturnsAsync(anotherProduct);

        var service = new ProductService(
            productRepository.Object,
            categoryRepository.Object);

        // Act
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.UpdateAsync(1, request));

        // Assert
        Assert.Equal(
            "Ya existe un producto con ese nombre.",
            exception.Message);

        productRepository.Verify(
            repository => repository.UpdateAsync(It.IsAny<Product>()),
            Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnTrue_WhenProductExists()
    {
        // Arrange
        var product = CreateProduct();

        var productRepository = new Mock<IProductRepository>();
        var categoryRepository = new Mock<ICategoryRepository>();

        productRepository
            .Setup(repository => repository.GetByIdAsync(1))
            .ReturnsAsync(product);

        productRepository
            .Setup(repository => repository.DeleteAsync(product))
            .Returns(Task.CompletedTask);

        var service = new ProductService(
            productRepository.Object,
            categoryRepository.Object);

        // Act
        var result = await service.DeleteAsync(1);

        // Assert
        Assert.True(result);

        productRepository.Verify(
            repository => repository.DeleteAsync(product),
            Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnFalse_WhenProductDoesNotExist()
    {
        // Arrange
        var productRepository = new Mock<IProductRepository>();
        var categoryRepository = new Mock<ICategoryRepository>();

        productRepository
            .Setup(repository => repository.GetByIdAsync(99))
            .ReturnsAsync((Product?)null);

        var service = new ProductService(
            productRepository.Object,
            categoryRepository.Object);

        // Act
        var result = await service.DeleteAsync(99);

        // Assert
        Assert.False(result);

        productRepository.Verify(
            repository => repository.DeleteAsync(It.IsAny<Product>()),
            Times.Never);
    }

    private static Product CreateProduct()
    {
        return new Product
        {
            Id = 1,
            Name = "Teclado Mecánico",
            Description = "Teclado para oficina",
            Price = 180000,
            Stock = 15,
            MinimumStock = 5,
            CategoryId = 1,
            Category = new Category
            {
                Id = 1,
                Name = "Hogar y Oficina"
            }
        };
    }
}