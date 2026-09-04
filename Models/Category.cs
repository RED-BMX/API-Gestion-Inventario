namespace API_Gestion_Inventario.Models;

public class Category
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Relación: una categoría puede tener muchos productos
    public ICollection<Product> Products { get; set; } = new List<Product>();
}