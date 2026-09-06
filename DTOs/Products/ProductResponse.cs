namespace API_Gestion_Inventario.DTOs.Products;

/// <summary>
/// Datos de respuesta de un producto.
/// </summary>
public class ProductResponse
{
    /// <summary>
    /// Identificador único del producto.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Nombre del producto.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Descripción del producto.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Precio actual del producto.
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// Cantidad actual disponible en inventario.
    /// </summary>
    public int Stock { get; set; }

    /// <summary>
    /// Cantidad mínima de unidades recomendada para mantener en inventario.
    /// </summary>
    public int MinimumStock { get; set; }

    /// <summary>
    /// Identificador de la categoría del producto.
    /// </summary>
    public int CategoryId { get; set; }

    /// <summary>
    /// Nombre de la categoría del producto.
    /// </summary>
    public string CategoryName { get; set; } = string.Empty;

    /// <summary>
    /// Fecha y hora de creación del producto.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Fecha y hora de la última actualización del producto.
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}
