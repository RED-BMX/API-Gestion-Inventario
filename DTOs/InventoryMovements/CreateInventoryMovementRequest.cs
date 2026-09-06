using System.ComponentModel.DataAnnotations;
using API_Gestion_Inventario.Models;

namespace API_Gestion_Inventario.DTOs.InventoryMovements;

/// <summary>
/// Datos necesarios para registrar un movimiento de inventario.
/// </summary>
public class CreateInventoryMovementRequest
{
    /// <summary>
    /// Identificador del producto afectado por el movimiento.
    /// </summary>
    [Range(1, int.MaxValue)]
    public int ProductId { get; set; }

    /// <summary>
    /// Tipo de movimiento que se realizará sobre el inventario.
    /// </summary>
    [Required]
    public MovementType Type { get; set; }

    /// <summary>
    /// Cantidad de unidades involucradas en el movimiento.
    /// </summary>
    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }

    /// <summary>
    /// Descripción opcional del movimiento.
    /// </summary>
    [StringLength(500)]
    public string? Description { get; set; }
}
