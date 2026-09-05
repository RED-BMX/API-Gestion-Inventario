using API_Gestion_Inventario.DTOs.Categories;
using API_Gestion_Inventario.Models;
using API_Gestion_Inventario.Repositories.Interfaces;
using API_Gestion_Inventario.Services.Interfaces;

namespace API_Gestion_Inventario.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;

    public CategoryService(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<IEnumerable<CategoryResponse>> GetAllAsync()
    {
        var categories = await _categoryRepository.GetAllAsync();

        return categories.Select(MapToResponse);
    }

    public async Task<CategoryResponse?> GetByIdAsync(int id)
    {
        var category = await _categoryRepository.GetByIdAsync(id);

        return category is null
            ? null
            : MapToResponse(category);
    }

    public async Task<CategoryResponse> CreateAsync(
        CreateCategoryRequest request)
    {
        var existingCategory = await _categoryRepository
            .GetByNameAsync(request.Name);

        if (existingCategory is not null)
        {
            throw new InvalidOperationException(
                "Ya existe una categoría con ese nombre.");
        }

        var category = new Category
        {
            Name = request.Name,
            Description = request.Description
        };

        await _categoryRepository.AddAsync(category);

        return MapToResponse(category);
    }

    public async Task<CategoryResponse?> UpdateAsync(
        int id,
        UpdateCategoryRequest request)
    {
        var existingCategory = await _categoryRepository
            .GetByIdAsync(id);

        if (existingCategory is null)
        {
            return null;
        }

        var categoryWithSameName = await _categoryRepository
            .GetByNameAsync(request.Name);

        if (categoryWithSameName is not null &&
            categoryWithSameName.Id != id)
        {
            throw new InvalidOperationException(
                "Ya existe una categoría con ese nombre.");
        }

        existingCategory.Name = request.Name;
        existingCategory.Description = request.Description;

        await _categoryRepository.UpdateAsync(existingCategory);

        return MapToResponse(existingCategory);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var category = await _categoryRepository.GetByIdAsync(id);

        if (category is null)
        {
            return false;
        }

        await _categoryRepository.DeleteAsync(category);

        return true;
    }

    private static CategoryResponse MapToResponse(Category category)
    {
        return new CategoryResponse
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            CreatedAt = category.CreatedAt
        };
    }
}
