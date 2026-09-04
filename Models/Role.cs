namespace API_Gestion_Inventario.Models;

public class Role
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    // Relación: un rol puede pertenecer a muchos usuarios
    public ICollection<User> Users { get; set; } = new List<User>();
}
