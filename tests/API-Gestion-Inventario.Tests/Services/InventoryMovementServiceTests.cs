using API_Gestion_Inventario.Data;
using API_Gestion_Inventario.DTOs.InventoryMovements;
using API_Gestion_Inventario.Models;
using API_Gestion_Inventario.Repositories.Interfaces;
using API_Gestion_Inventario.Services;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace API_Gestion_Inventario.Tests.Services;

public class InventoryMovementServiceTests
{
    [Fact]
    public async Task GetAllAsync_ShouldReturnMovements()
    {
        // Arrange
        var product = CreateProduct();

        var movements = new List<InventoryMovement>
        {
            new()
            {
                Id = 1,
                ProductId = product.Id,
                Product = product,
                Type = MovementType.IN,
                Quantity = 5,
                Description = "Entrada de mercancía"
            },
            new()
            {
                Id = 2,
                ProductId = product.Id,
                Product = product,
                Type = MovementType.OUT,
                Quantity = 2,
                Description = "Salida de mercancía"
            }
        };

        var movementRepository =
            new Mock<IInventoryMovementRepository>();

        var productRepository =
            new Mock<IProductRepository>();

        movementRepository
            .Setup(repository => repository.GetAllAsync())
            .ReturnsAsync(movements);

        await using var context = CreateContext();

        var service = new InventoryMovementService(
            movementRepository.Object,
            productRepository.Object,
            context);

        // Act
        var result = (await service.GetAllAsync()).ToList();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Equal("IN", result[0].Type);
        Assert.Equal("OUT", result[1].Type);
        Assert.Equal("Teclado Mecánico", result[0].ProductName);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnMovement_WhenMovementExists()
    {
        // Arrange
        var product = CreateProduct();

        var movement = new InventoryMovement
        {
            Id = 1,
            ProductId = product.Id,
            Product = product,
            Type = MovementType.IN,
            Quantity = 5,
            Description = "Entrada de mercancía"
        };

        var movementRepository =
            new Mock<IInventoryMovementRepository>();

        var productRepository =
            new Mock<IProductRepository>();

        movementRepository
            .Setup(repository => repository.GetByIdAsync(1))
            .ReturnsAsync(movement);

        await using var context = CreateContext();

        var service = new InventoryMovementService(
            movementRepository.Object,
            productRepository.Object,
            context);

        // Act
        var result = await service.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal(1, result.ProductId);
        Assert.Equal("Teclado Mecánico", result.ProductName);
        Assert.Equal("IN", result.Type);
        Assert.Equal(5, result.Quantity);
        Assert.Equal("Entrada de mercancía", result.Description);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenMovementDoesNotExist()
    {
        // Arrange
        var movementRepository =
            new Mock<IInventoryMovementRepository>();

        var productRepository =
            new Mock<IProductRepository>();

        movementRepository
            .Setup(repository => repository.GetByIdAsync(99))
            .ReturnsAsync((InventoryMovement?)null);

        await using var context = CreateContext();

        var service = new InventoryMovementService(
            movementRepository.Object,
            productRepository.Object,
            context);

        // Act
        var result = await service.GetByIdAsync(99);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateAsync_ShouldIncreaseStock_WhenMovementIsIN()
    {
        // Arrange
        var product = CreateProduct();
        product.Stock = 10;

        var request = new CreateInventoryMovementRequest
        {
            ProductId = 1,
            Type = MovementType.IN,
            Quantity = 5,
            Description = "Entrada de mercancía"
        };

        var movementRepository =
            new Mock<IInventoryMovementRepository>();

        var productRepository =
            new Mock<IProductRepository>();

        productRepository
            .Setup(repository => repository.GetByIdAsync(1))
            .ReturnsAsync(product);

        productRepository
            .Setup(repository => repository.UpdateAsync(
                It.IsAny<Product>()))
            .Returns(Task.CompletedTask);

        movementRepository
            .Setup(repository => repository.AddAsync(
                It.IsAny<InventoryMovement>()))
            .Returns(Task.CompletedTask);

        await using var context = CreateContext();

        var service = new InventoryMovementService(
            movementRepository.Object,
            productRepository.Object,
            context);

        // Act
        var result = await service.CreateAsync(request);

        // Assert
        Assert.Equal(15, product.Stock);
        Assert.Equal("IN", result.Type);
        Assert.Equal(5, result.Quantity);
        Assert.Equal("Entrada de mercancía", result.Description);

        productRepository.Verify(
            repository => repository.UpdateAsync(
                It.Is<Product>(p => p.Stock == 15)),
            Times.Once);

        movementRepository.Verify(
            repository => repository.AddAsync(
                It.Is<InventoryMovement>(m =>
                    m.ProductId == 1 &&
                    m.Type == MovementType.IN &&
                    m.Quantity == 5)),
            Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ShouldDecreaseStock_WhenMovementIsOUT()
    {
        // Arrange
        var product = CreateProduct();
        product.Stock = 10;

        var request = new CreateInventoryMovementRequest
        {
            ProductId = 1,
            Type = MovementType.OUT,
            Quantity = 3,
            Description = "Salida de mercancía"
        };

        var movementRepository =
            new Mock<IInventoryMovementRepository>();

        var productRepository =
            new Mock<IProductRepository>();

        productRepository
            .Setup(repository => repository.GetByIdAsync(1))
            .ReturnsAsync(product);

        productRepository
            .Setup(repository => repository.UpdateAsync(
                It.IsAny<Product>()))
            .Returns(Task.CompletedTask);

        movementRepository
            .Setup(repository => repository.AddAsync(
                It.IsAny<InventoryMovement>()))
            .Returns(Task.CompletedTask);

        await using var context = CreateContext();

        var service = new InventoryMovementService(
            movementRepository.Object,
            productRepository.Object,
            context);

        // Act
        var result = await service.CreateAsync(request);

        // Assert
        Assert.Equal(7, product.Stock);
        Assert.Equal("OUT", result.Type);
        Assert.Equal(3, result.Quantity);

        productRepository.Verify(
            repository => repository.UpdateAsync(
                It.Is<Product>(p => p.Stock == 7)),
            Times.Once);

        movementRepository.Verify(
            repository => repository.AddAsync(
                It.Is<InventoryMovement>(m =>
                    m.ProductId == 1 &&
                    m.Type == MovementType.OUT &&
                    m.Quantity == 3)),
            Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ShouldThrow_WhenOUTQuantityExceedsStock()
    {
        // Arrange
        var product = CreateProduct();
        product.Stock = 5;

        var request = new CreateInventoryMovementRequest
        {
            ProductId = 1,
            Type = MovementType.OUT,
            Quantity = 10
        };

        var movementRepository =
            new Mock<IInventoryMovementRepository>();

        var productRepository =
            new Mock<IProductRepository>();

        productRepository
            .Setup(repository => repository.GetByIdAsync(1))
            .ReturnsAsync(product);

        await using var context = CreateContext();

        var service = new InventoryMovementService(
            movementRepository.Object,
            productRepository.Object,
            context);

        // Act
        var exception =
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => service.CreateAsync(request));

        // Assert
        Assert.Equal(
            "No hay stock suficiente para realizar la salida.",
            exception.Message);

        Assert.Equal(5, product.Stock);

        productRepository.Verify(
            repository => repository.UpdateAsync(
                It.IsAny<Product>()),
            Times.Never);

        movementRepository.Verify(
            repository => repository.AddAsync(
                It.IsAny<InventoryMovement>()),
            Times.Never);
    }

    [Fact]
    public async Task CreateAsync_ShouldSetStock_WhenMovementIsADJUSTMENT()
    {
        // Arrange
        var product = CreateProduct();
        product.Stock = 20;

        var request = new CreateInventoryMovementRequest
        {
            ProductId = 1,
            Type = MovementType.ADJUSTMENT,
            Quantity = 8,
            Description = "Ajuste de inventario"
        };

        var movementRepository =
            new Mock<IInventoryMovementRepository>();

        var productRepository =
            new Mock<IProductRepository>();

        productRepository
            .Setup(repository => repository.GetByIdAsync(1))
            .ReturnsAsync(product);

        productRepository
            .Setup(repository => repository.UpdateAsync(
                It.IsAny<Product>()))
            .Returns(Task.CompletedTask);

        movementRepository
            .Setup(repository => repository.AddAsync(
                It.IsAny<InventoryMovement>()))
            .Returns(Task.CompletedTask);

        await using var context = CreateContext();

        var service = new InventoryMovementService(
            movementRepository.Object,
            productRepository.Object,
            context);

        // Act
        var result = await service.CreateAsync(request);

        // Assert
        Assert.Equal(8, product.Stock);
        Assert.Equal("ADJUSTMENT", result.Type);
        Assert.Equal(8, result.Quantity);

        productRepository.Verify(
            repository => repository.UpdateAsync(
                It.Is<Product>(p => p.Stock == 8)),
            Times.Once);

        movementRepository.Verify(
            repository => repository.AddAsync(
                It.Is<InventoryMovement>(m =>
                    m.Type == MovementType.ADJUSTMENT &&
                    m.Quantity == 8)),
            Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ShouldThrow_WhenProductDoesNotExist()
    {
        // Arrange
        var request = new CreateInventoryMovementRequest
        {
            ProductId = 99,
            Type = MovementType.IN,
            Quantity = 5
        };

        var movementRepository =
            new Mock<IInventoryMovementRepository>();

        var productRepository =
            new Mock<IProductRepository>();

        productRepository
            .Setup(repository => repository.GetByIdAsync(99))
            .ReturnsAsync((Product?)null);

        await using var context = CreateContext();

        var service = new InventoryMovementService(
            movementRepository.Object,
            productRepository.Object,
            context);

        // Act
        var exception =
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => service.CreateAsync(request));

        // Assert
        Assert.Equal(
            "El producto especificado no existe.",
            exception.Message);

        productRepository.Verify(
            repository => repository.UpdateAsync(
                It.IsAny<Product>()),
            Times.Never);

        movementRepository.Verify(
            repository => repository.AddAsync(
                It.IsAny<InventoryMovement>()),
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
            Stock = 10,
            MinimumStock = 5,
            CategoryId = 1,
            Category = new Category
            {
                Id = 1,
                Name = "Hogar y Oficina"
            }
        };
    }

    private static ApplicationDbContext CreateContext()
    {
        var connection = new Microsoft.Data.Sqlite.SqliteConnection(
            "DataSource=:memory:");
    
        connection.Open();
    
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(connection)
            .Options;
    
        var context = new ApplicationDbContext(options);
    
        context.Database.EnsureCreated();
    
        return context;
    }
}