using API_Gestion_Inventario.DTOs.InventoryMovements;
using API_Gestion_Inventario.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API_Gestion_Inventario.Controllers;

/// <summary>
/// Gestiona las operaciones de consulta y registro de movimientos de inventario.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class InventoryMovementsController : ControllerBase
{
    private readonly IInventoryMovementService _movementService;

    public InventoryMovementsController(
        IInventoryMovementService movementService)
    {
        _movementService = movementService;
    }

    /// <summary>
    /// Obtiene todos los movimientos de inventario registrados.
    /// </summary>
    /// <returns>
    /// Lista de movimientos de inventario.
    /// </returns>
    /// <response code="200">
    /// Movimientos obtenidos correctamente.
    /// </response>
    /// <response code="401">
    /// El usuario no está autenticado.
    /// </response>
    [HttpGet]
    [ProducesResponseType(
        typeof(IEnumerable<InventoryMovementResponse>),
        StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IEnumerable<InventoryMovementResponse>>> GetAll()
    {
        var movements = await _movementService.GetAllAsync();

        return Ok(movements);
    }

    /// <summary>
    /// Obtiene un movimiento de inventario mediante su identificador.
    /// </summary>
    /// <param name="id">Identificador del movimiento.</param>
    /// <returns>
    /// Información del movimiento solicitado.
    /// </returns>
    /// <response code="200">
    /// Movimiento encontrado correctamente.
    /// </response>
    /// <response code="401">
    /// El usuario no está autenticado.
    /// </response>
    /// <response code="404">
    /// El movimiento no existe.
    /// </response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(
        typeof(InventoryMovementResponse),
        StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<InventoryMovementResponse>> GetById(int id)
    {
        var movement = await _movementService.GetByIdAsync(id);

        if (movement is null)
        {
            return NotFound(new
            {
                message = "El movimiento no existe."
            });
        }

        return Ok(movement);
    }

    /// <summary>
    /// Registra un nuevo movimiento de inventario y actualiza el stock del producto.
    /// </summary>
    /// <param name="request">
    /// Datos del movimiento, incluyendo producto, tipo, cantidad y descripción.
    /// </param>
    /// <returns>
    /// El movimiento de inventario creado.
    /// </returns>
    /// <response code="201">
    /// Movimiento registrado correctamente.
    /// </response>
    /// <response code="400">
    /// Los datos enviados no cumplen con las validaciones requeridas.
    /// </response>
    /// <response code="401">
    /// El usuario no está autenticado.
    /// </response>
    /// <response code="403">
    /// El usuario autenticado no tiene el rol Admin.
    /// </response>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(
        typeof(InventoryMovementResponse),
        StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<InventoryMovementResponse>> Create(
        CreateInventoryMovementRequest request)
    {
        var movement =
            await _movementService.CreateAsync(request);

        return CreatedAtAction(
            nameof(GetById),
            new { id = movement.Id },
            movement);
    }
}