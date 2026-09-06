namespace API_Gestion_Inventario.DTOs.InventoryMovements;

/// <summary>
/// Datos de respuesta de un movimiento de inventario.
/// </summary>
public class InventoryMovementResponse
{
    /// <summary>
    /// Identificador único del movimiento.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Identificador del producto afectado.
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    /// Nombre del producto afectado.
    /// </summary>
    public string ProductName { get; set; } = string.Empty;

    /// <summary>
    /// Tipo de movimiento registrado.
    /// </summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Cantidad de unidades involucradas en el movimiento.
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Fecha y hora en que se registró el movimiento.
    /// </summary>
    public DateTime Date { get; set; }

    /// <summary>
    /// Descripción del movimiento.
    /// </summary>
    public string? Description { get; set; }
}
