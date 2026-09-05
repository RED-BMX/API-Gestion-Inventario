namespace API_Gestion_Inventario.DTOs.InventoryMovements;

public class InventoryMovementResponse
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public DateTime Date { get; set; }
    public string? Description { get; set; }
}
