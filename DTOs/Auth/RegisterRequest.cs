using System.ComponentModel.DataAnnotations;

namespace API_Gestion_Inventario.DTOs.Auth;

/// <summary>
/// Datos necesarios para registrar un nuevo usuario.
/// </summary>
public class RegisterRequest
{
    /// <summary>
    /// Nombre de usuario que tendrá el nuevo usuario.
    /// </summary>
    [Required]
    [StringLength(50, MinimumLength = 3)]
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Correo electrónico del nuevo usuario.
    /// </summary>
    [Required]
    [EmailAddress]
    [StringLength(254)]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Contraseña del nuevo usuario.
    /// </summary>
    [Required]
    [StringLength(100, MinimumLength = 8)]
    public string Password { get; set; } = string.Empty;
}
