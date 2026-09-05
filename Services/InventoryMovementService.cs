using API_Gestion_Inventario.DTOs.InventoryMovements;
using API_Gestion_Inventario.Models;
using API_Gestion_Inventario.Repositories.Interfaces;
using API_Gestion_Inventario.Services.Interfaces;
using API_Gestion_Inventario.Data;

namespace API_Gestion_Inventario.Services;

public class InventoryMovementService : IInventoryMovementService
{
    private readonly IInventoryMovementRepository _movementRepository;
    private readonly IProductRepository _productRepository;
    private readonly ApplicationDbContext _context;

    public InventoryMovementService(
        IInventoryMovementRepository movementRepository,
        IProductRepository productRepository,
        ApplicationDbContext context)
    {
        _movementRepository = movementRepository;
        _productRepository = productRepository;
        _context = context;
    }

    public async Task<IEnumerable<InventoryMovementResponse>> GetAllAsync()
    {
        var movements = await _movementRepository.GetAllAsync();

        return movements.Select(MapToResponse);
    }

    public async Task<InventoryMovementResponse?> GetByIdAsync(int id)
    {
        var movement = await _movementRepository.GetByIdAsync(id);

        if (movement is null)
        {
            return null;
        }

        return MapToResponse(movement);
    }

    public async Task<InventoryMovementResponse> CreateAsync(
        CreateInventoryMovementRequest request)
    {
        var product = await _productRepository
            .GetByIdAsync(request.ProductId);
    
        if (product is null)
        {
            throw new InvalidOperationException(
                "El producto especificado no existe.");
        }
    
        switch (request.Type)
        {
            case MovementType.IN:
                product.Stock += request.Quantity;
                break;
    
            case MovementType.OUT:
                if (request.Quantity > product.Stock)
                {
                    throw new InvalidOperationException(
                        "No hay stock suficiente para realizar la salida.");
                }
    
                product.Stock -= request.Quantity;
                break;
    
            case MovementType.ADJUSTMENT:
                product.Stock = request.Quantity;
                break;
    
            default:
                throw new InvalidOperationException(
                    "El tipo de movimiento no es válido.");
        }
    
        product.UpdatedAt = DateTime.UtcNow;
    
        var movement = new InventoryMovement
        {
            ProductId = product.Id,
            Type = request.Type,
            Quantity = request.Quantity,
            Description = request.Description,
            Date = DateTime.UtcNow,
            Product = product
        };
    
        await using var transaction =
            await _context.Database.BeginTransactionAsync();
    
        try
        {
            await _productRepository.UpdateAsync(product);
            await _movementRepository.AddAsync(movement);
    
            await transaction.CommitAsync();
    
            return MapToResponse(movement);
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    private static InventoryMovementResponse MapToResponse(
        InventoryMovement movement)
    {
        return new InventoryMovementResponse
        {
            Id = movement.Id,
            ProductId = movement.ProductId,
            ProductName = movement.Product?.Name ?? string.Empty,
            Type = movement.Type.ToString(),
            Quantity = movement.Quantity,
            Date = movement.Date,
            Description = movement.Description
        };
    }
}
