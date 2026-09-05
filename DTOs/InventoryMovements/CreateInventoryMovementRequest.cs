using System.ComponentModel.DataAnnotations;
using API_Gestion_Inventario.Models;

namespace API_Gestion_Inventario.DTOs.InventoryMovements;

public class CreateInventoryMovementRequest
{
    [Range(1, int.MaxValue)]
    public int ProductId { get; set; }

    [Required]
    public MovementType Type { get; set; }

    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }

    [StringLength(500)]
    public string? Description { get; set; }
}
