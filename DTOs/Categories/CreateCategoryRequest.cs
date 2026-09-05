using System.ComponentModel.DataAnnotations;

namespace API_Gestion_Inventario.DTOs.Categories;

public class CreateCategoryRequest
{
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }
}
