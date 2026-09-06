using System.ComponentModel.DataAnnotations;

namespace API_Gestion_Inventario.DTOs.Products;

/// <summary>
/// Datos necesarios para actualizar un producto existente.
/// </summary>
public class UpdateProductRequest
{
    /// <summary>
    /// Nombre del producto.
    /// </summary>
    [Required]
    [StringLength(150, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Descripción opcional del producto.
    /// </summary>
    [StringLength(500)]
    public string? Description { get; set; }

    /// <summary>
    /// Precio del producto.
    /// </summary>
    [Range(0, 99999999.99)]
    public decimal Price { get; set; }

    /// <summary>
    /// Cantidad actual disponible en inventario.
    /// </summary>
    [Range(0, int.MaxValue)]
    public int Stock { get; set; }

    /// <summary>
    /// Cantidad mínima de unidades recomendada para mantener en inventario.
    /// </summary>
    [Range(0, int.MaxValue)]
    public int MinimumStock { get; set; }

    /// <summary>
    /// Identificador de la categoría a la que pertenece el producto.
    /// </summary>
    [Range(1, int.MaxValue)]
    public int CategoryId { get; set; }
}
