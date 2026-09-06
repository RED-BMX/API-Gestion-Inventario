using System.ComponentModel.DataAnnotations;

namespace API_Gestion_Inventario.DTOs.Categories;

/// <summary>
/// Datos necesarios para actualizar una categoría existente.
/// </summary>
public class UpdateCategoryRequest
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
