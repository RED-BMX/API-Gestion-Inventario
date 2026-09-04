namespace API_Gestion_Inventario.Models;

public class InventoryMovement
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public MovementType Type { get; set; }

    public int Quantity { get; set; }

    public DateTime Date { get; set; } = DateTime.UtcNow;

    public string? Description { get; set; }

    // Relación: muchos movimientos pertenecen a un producto
    public Product Product { get; set; } = null!;
}
