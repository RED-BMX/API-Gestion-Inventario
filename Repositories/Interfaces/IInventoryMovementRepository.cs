using API_Gestion_Inventario.Models;

namespace API_Gestion_Inventario.Repositories.Interfaces;

public interface IInventoryMovementRepository
{
    Task<IEnumerable<InventoryMovement>> GetAllAsync();
    Task<InventoryMovement?> GetByIdAsync(int id);
    Task AddAsync(InventoryMovement movement);
}