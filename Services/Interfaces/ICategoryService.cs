using API_Gestion_Inventario.DTOs.Categories;

namespace API_Gestion_Inventario.Services.Interfaces;

public interface ICategoryService
{
    Task<IEnumerable<CategoryResponse>> GetAllAsync();

    Task<CategoryResponse?> GetByIdAsync(int id);

    Task<CategoryResponse> CreateAsync(CreateCategoryRequest request);

    Task<CategoryResponse?> UpdateAsync(
        int id,
        UpdateCategoryRequest request);

    Task<bool> DeleteAsync(int id);
}
