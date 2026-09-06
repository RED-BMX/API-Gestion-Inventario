using System.ComponentModel.DataAnnotations;

namespace API_Gestion_Inventario.DTOs.Auth;

/// <summary>
/// Datos necesarios para iniciar sesión.
/// </summary>
public class LoginRequest
{
    /// <summary>
    /// Nombre de usuario registrado.
    /// </summary>
    [Required]
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Contraseña del usuario.
    /// </summary>
    [Required]
    public string Password { get; set; } = string.Empty;
}
