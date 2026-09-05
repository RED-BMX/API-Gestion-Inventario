using API_Gestion_Inventario.Data;
using API_Gestion_Inventario.Models;
using API_Gestion_Inventario.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API_Gestion_Inventario.Repositories;

public class InventoryMovementRepository : IInventoryMovementRepository
{
    private readonly ApplicationDbContext _context;

    public InventoryMovementRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<InventoryMovement>> GetAllAsync()
    {
        return await _context.InventoryMovements
            .AsNoTracking()
            .Include(m => m.Product)
            .OrderByDescending(m => m.Date)
            .ToListAsync();
    }

    public async Task<InventoryMovement?> GetByIdAsync(int id)
    {
        return await _context.InventoryMovements
            .AsNoTracking()
            .Include(m => m.Product)
            .FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task AddAsync(InventoryMovement movement)
    {
        await _context.InventoryMovements.AddAsync(movement);
        await _context.SaveChangesAsync();
    }
}

