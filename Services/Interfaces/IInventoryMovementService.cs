using API_Gestion_Inventario.DTOs.InventoryMovements;

namespace API_Gestion_Inventario.Services.Interfaces;

public interface IInventoryMovementService
{
    Task<IEnumerable<InventoryMovementResponse>> GetAllAsync();
    Task<InventoryMovementResponse?> GetByIdAsync(int id);
    Task<InventoryMovementResponse> CreateAsync(
        CreateInventoryMovementRequest request);
}
