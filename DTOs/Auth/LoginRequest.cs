using System.ComponentModel.DataAnnotations;

namespace API_Gestion_Inventario.DTOs.Auth;

public class LoginRequest
{
    [Required]
    public string Username { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}