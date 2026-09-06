namespace API_Gestion_Inventario.DTOs.Auth;

/// <summary>
/// Respuesta generada después de una autenticación exitosa.
/// </summary>
public class AuthResponse
{
    /// <summary>
    /// Token JWT utilizado para autenticar las solicitudes protegidas.
    /// </summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// Identificador único del usuario autenticado.
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// Nombre de usuario.
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Correo electrónico del usuario.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Rol asignado al usuario.
    /// </summary>
    public string Role { get; set; } = string.Empty;
}
