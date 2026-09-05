using API_Gestion_Inventario.DTOs.InventoryMovements;
using API_Gestion_Inventario.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API_Gestion_Inventario.Controllers;

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

    [HttpGet]
    public async Task<ActionResult<IEnumerable<InventoryMovementResponse>>> GetAll()
    {
        var movements = await _movementService.GetAllAsync();

        return Ok(movements);
    }

    [HttpGet("{id:int}")]
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

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<InventoryMovementResponse>> Create(
        CreateInventoryMovementRequest request)
    {
        try
        {
            var movement =
                await _movementService.CreateAsync(request);

            return CreatedAtAction(
                nameof(GetById),
                new { id = movement.Id },
                movement);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
    }
}
