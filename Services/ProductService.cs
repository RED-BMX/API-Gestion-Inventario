using API_Gestion_Inventario.DTOs.Products;
using API_Gestion_Inventario.Models;
using API_Gestion_Inventario.Repositories.Interfaces;
using API_Gestion_Inventario.Services.Interfaces;

namespace API_Gestion_Inventario.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;

    public ProductService(
        IProductRepository productRepository,
        ICategoryRepository categoryRepository)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
    }

    public async Task<IEnumerable<ProductResponse>> GetAllAsync()
    {
        var products = await _productRepository.GetAllAsync();

        return products.Select(MapToResponse);
    }

    public async Task<ProductResponse?> GetByIdAsync(int id)
    {
        var product = await _productRepository.GetByIdAsync(id);

        if (product is null)
        {
            return null;
        }

        return MapToResponse(product);
    }

    public async Task<ProductResponse> CreateAsync(
        CreateProductRequest request)
    {
        var existingProduct = await _productRepository
            .GetByNameAsync(request.Name);

        if (existingProduct is not null)
        {
            throw new InvalidOperationException(
                "Ya existe un producto con ese nombre.");
        }

        var category = await _categoryRepository
            .GetByIdAsync(request.CategoryId);

        if (category is null)
        {
            throw new InvalidOperationException(
                "La categoría especificada no existe.");
        }

        var product = new Product
        {
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            Stock = request.Stock,
            MinimumStock = request.MinimumStock,
            CategoryId = request.CategoryId
        };

        await _productRepository.AddAsync(product);

        product.Category = category;

        return MapToResponse(product);
    }

    public async Task<ProductResponse?> UpdateAsync(
        int id,
        UpdateProductRequest request)
    {
        var existingProduct = await _productRepository
            .GetByIdAsync(id);

        if (existingProduct is null)
        {
            return null;
        }

        var category = await _categoryRepository
            .GetByIdAsync(request.CategoryId);

        if (category is null)
        {
            throw new InvalidOperationException(
                "La categoría especificada no existe.");
        }

        var productWithSameName = await _productRepository
            .GetByNameAsync(request.Name);

        if (productWithSameName is not null &&
            productWithSameName.Id != id)
        {
            throw new InvalidOperationException(
                "Ya existe un producto con ese nombre.");
        }

        existingProduct.Name = request.Name;
        existingProduct.Description = request.Description;
        existingProduct.Price = request.Price;
        existingProduct.Stock = request.Stock;
        existingProduct.MinimumStock = request.MinimumStock;
        existingProduct.CategoryId = request.CategoryId;
        existingProduct.UpdatedAt = DateTime.UtcNow;

        await _productRepository.UpdateAsync(existingProduct);

        existingProduct.Category = category;

        return MapToResponse(existingProduct);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var product = await _productRepository.GetByIdAsync(id);

        if (product is null)
        {
            return false;
        }

        await _productRepository.DeleteAsync(product);

        return true;
    }

    private static ProductResponse MapToResponse(Product product)
    {
        return new ProductResponse
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            Stock = product.Stock,
            MinimumStock = product.MinimumStock,
            CategoryId = product.CategoryId,
            CategoryName = product.Category?.Name ?? string.Empty,
            CreatedAt = product.CreatedAt,
            UpdatedAt = product.UpdatedAt
        };
    }
}