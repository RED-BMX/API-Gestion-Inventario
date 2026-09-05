using System.ComponentModel.DataAnnotations;

namespace API_Gestion_Inventario.DTOs.Products;

public class CreateProductRequest
{
    [Required]
    [StringLength(150, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    [Range(0, 99999999.99)]
    public decimal Price { get; set; }

    [Range(0, int.MaxValue)]
    public int Stock { get; set; }

    [Range(0, int.MaxValue)]
    public int MinimumStock { get; set; }

    [Range(1, int.MaxValue)]
    public int CategoryId { get; set; }
}
