using System.ComponentModel.DataAnnotations;

namespace API_Gestion_Inventario.DTOs.Categories;

/// <summary>
/// Datos necesarios para crear una nueva categoría.
/// </summary>
public class CreateCategoryRequest
{
    /// <summary>
    /// Nombre de la categoría.
    /// </summary>
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Descripción opcional de la categoría.
    /// </summary>
    [StringLength(500)]
    public string? Description { get; set; }
}
