namespace API_Gestion_Inventario.Models;

public class User
{
    public int Id { get; set; }

    public string Username { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public int RoleId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Relación: muchos usuarios pertenecen a un rol
    public Role Role { get; set; } = null!;
}
