using API_Gestion_Inventario.Models;

namespace API_Gestion_Inventario.Services.Interfaces;

public interface IJwtService
{
    string GenerateToken(User user);
}