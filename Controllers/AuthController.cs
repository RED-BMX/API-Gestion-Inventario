using API_Gestion_Inventario.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using API_Gestion_Inventario.DTOs.Auth;

namespace API_Gestion_Inventario.Controllers;

/// <summary>
/// Gestiona el registro y autenticación de usuarios.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Registra un nuevo usuario en el sistema.
    /// </summary>
    /// <param name="request">
    /// Datos necesarios para crear la cuenta, incluyendo nombre de usuario,
    /// correo electrónico y contraseña.
    /// </param>
    /// <returns>
    /// Información del usuario registrado y un token JWT.
    /// </returns>
    /// <response code="200">
    /// Usuario registrado correctamente.
    /// </response>
    /// <response code="400">
    /// Los datos enviados no cumplen con las validaciones requeridas.
    /// </response>
    /// <response code="409">
    /// Ya existe un usuario con el mismo nombre de usuario o correo electrónico.
    /// </response>
    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<AuthResponse>> Register(
        RegisterRequest request)
    {
        try
        {
            var response = await _authService.RegisterAsync(request);

            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new
            {
                message = ex.Message
            });
        }
    }

    /// <summary>
    /// Autentica un usuario y genera un token JWT.
    /// </summary>
    /// <param name="request">
    /// Credenciales del usuario.
    /// </param>
    /// <returns>
    /// Información del usuario autenticado y un token JWT.
    /// </returns>
    /// <response code="200">
    /// Autenticación exitosa.
    /// </response>
    /// <response code="400">
    /// Las credenciales no cumplen con las validaciones requeridas.
    /// </response>
    /// <response code="401">
    /// El nombre de usuario o la contraseña son incorrectos.
    /// </response>
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponse>> Login(
        LoginRequest request)
    {
        try
        {
            var response = await _authService.LoginAsync(request);

            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new
            {
                message = ex.Message
            });
        }
    }
}