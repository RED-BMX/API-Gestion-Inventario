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

    public async Task<IEnumerable<Category>> GetAllAsync()
    {
        return await _categoryRepository.GetAllAsync();
    }

    public async Task<Category?> GetByIdAsync(int id)
    {
        return await _categoryRepository.GetByIdAsync(id);
    }

    public async Task<Category> CreateAsync(Category category)
    {
        var existingCategory = await _categoryRepository
            .GetByNameAsync(category.Name);

        if (existingCategory is not null)
        {
            throw new InvalidOperationException(
                "Ya existe una categoría con ese nombre.");
        }

        await _categoryRepository.AddAsync(category);

        return category;
    }

    public async Task<Category?> UpdateAsync(
        int id,
        Category category)
    {
        var existingCategory = await _categoryRepository
            .GetByIdAsync(id);

        if (existingCategory is null)
        {
            return null;
        }

        var categoryWithSameName = await _categoryRepository
            .GetByNameAsync(category.Name);

        if (categoryWithSameName is not null &&
            categoryWithSameName.Id != id)
        {
            throw new InvalidOperationException(
                "Ya existe una categoría con ese nombre.");
        }

        existingCategory.Name = category.Name;
        existingCategory.Description = category.Description;

        await _categoryRepository.UpdateAsync(existingCategory);

        return existingCategory;
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
}
