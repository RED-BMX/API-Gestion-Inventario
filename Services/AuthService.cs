using API_Gestion_Inventario.Data;
using API_Gestion_Inventario.DTOs.Auth;
using API_Gestion_Inventario.Models;
using API_Gestion_Inventario.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API_Gestion_Inventario.Services;

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _context;
    private readonly IPasswordService _passwordService;
    private readonly IJwtService _jwtService;

    public AuthService(
        ApplicationDbContext context,
        IPasswordService passwordService,
        IJwtService jwtService)
    {
        _context = context;
        _passwordService = passwordService;
        _jwtService = jwtService;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        var usernameExists = await _context.Users
            .AnyAsync(u => u.Username == request.Username);
    
        if (usernameExists)
        {
            throw new InvalidOperationException(
                "El nombre de usuario ya está registrado.");
        }
    
        var emailExists = await _context.Users
            .AnyAsync(u => u.Email == request.Email);
    
        if (emailExists)
        {
            throw new InvalidOperationException(
                "El correo electrónico ya está registrado.");
        }
    
        var roleExists = await _context.Roles
            .AnyAsync(r => r.Id == request.RoleId);
    
        if (!roleExists)
        {
            throw new InvalidOperationException(
                "El rol especificado no existe.");
        }
    
        var user = new User
        {
            Username = request.Username,
            Email = request.Email,
            PasswordHash = _passwordService.HashPassword(request.Password),
            RoleId = request.RoleId
        };
    
        _context.Users.Add(user);
    
        await _context.SaveChangesAsync();
    
        var role = await _context.Roles
            .FirstAsync(r => r.Id == user.RoleId);
    
        user.Role = role;
        
        var token = _jwtService.GenerateToken(user);
        
        return new AuthResponse
        {
            Token = token,
            UserId = user.Id,
            Username = user.Username,
            Email = user.Email,
            Role = role.Name
        };
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var user = await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Username == request.Username);
    
        if (user is null)
        {
            throw new UnauthorizedAccessException(
                "Credenciales inválidas.");
        }
    
        var passwordValid = _passwordService.VerifyPassword(
            request.Password,
            user.PasswordHash);
    
        if (!passwordValid)
        {
            throw new UnauthorizedAccessException(
                "Credenciales inválidas.");
        }
    
        var token = _jwtService.GenerateToken(user);
    
        return new AuthResponse
        {
            Token = token,
            UserId = user.Id,
            Username = user.Username,
            Email = user.Email,
            Role = user.Role.Name
        };
    }
}