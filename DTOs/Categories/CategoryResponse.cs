namespace API_Gestion_Inventario.DTOs.Categories;

/// <summary>
/// Datos de respuesta de una categoría.
/// </summary>
public class CategoryResponse
{
    /// <summary>
    /// Identificador único de la categoría.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Nombre de la categoría.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Descripción de la categoría.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Fecha y hora de creación de la categoría.
    /// </summary>
    public DateTime CreatedAt { get; set; }
}
