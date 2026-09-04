using API_Gestion_Inventario.DTOs.Auth;

namespace API_Gestion_Inventario.Services.Interfaces;

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request);

    Task<AuthResponse> LoginAsync(LoginRequest request);
}