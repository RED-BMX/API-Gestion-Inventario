using API_Gestion_Inventario.Models;

namespace API_Gestion_Inventario.Services.Interfaces;

public interface ICategoryService
{
    Task<IEnumerable<Category>> GetAllAsync();

    Task<Category?> GetByIdAsync(int id);

    Task<Category> CreateAsync(Category category);

    Task<Category?> UpdateAsync(int id, Category category);

    Task<bool> DeleteAsync(int id);
}
