using API_Gestion_Inventario.DTOs.Categories;
using API_Gestion_Inventario.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API_Gestion_Inventario.Controllers;

/// <summary>
/// Gestiona las operaciones de consulta y administración de categorías.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoriesController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    /// <summary>
    /// Obtiene todas las categorías registradas.
    /// </summary>
    /// <returns>
    /// Lista de categorías.
    /// </returns>
    /// <response code="200">
    /// Categorías obtenidas correctamente.
    /// </response>
    /// <response code="401">
    /// El usuario no está autenticado.
    /// </response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<CategoryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IEnumerable<CategoryResponse>>> GetAll()
    {
        var categories = await _categoryService.GetAllAsync();

        return Ok(categories);
    }

    /// <summary>
    /// Obtiene una categoría mediante su identificador.
    /// </summary>
    /// <param name="id">Identificador de la categoría.</param>
    /// <returns>
    /// Información de la categoría solicitada.
    /// </returns>
    /// <response code="200">
    /// Categoría encontrada correctamente.
    /// </response>
    /// <response code="401">
    /// El usuario no está autenticado.
    /// </response>
    /// <response code="404">
    /// La categoría no existe.
    /// </response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(CategoryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CategoryResponse>> GetById(int id)
    {
        var category = await _categoryService.GetByIdAsync(id);

        if (category is null)
        {
            return NotFound(new
            {
                message = "La categoría no existe."
            });
        }

        return Ok(category);
    }

    /// <summary>
    /// Crea una nueva categoría.
    /// </summary>
    /// <param name="request">
    /// Datos de la categoría que se desea crear.
    /// </param>
    /// <returns>
    /// La categoría creada.
    /// </returns>
    /// <response code="201">
    /// Categoría creada correctamente.
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
    /// <response code="409">
    /// Ya existe una categoría con el mismo nombre.
    /// </response>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(CategoryResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CategoryResponse>> Create(
        CreateCategoryRequest request)
    {
        try
        {
            var createdCategory =
                await _categoryService.CreateAsync(request);

            return CreatedAtAction(
                nameof(GetById),
                new { id = createdCategory.Id },
                createdCategory);
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
    /// Actualiza una categoría existente.
    /// </summary>
    /// <param name="id">Identificador de la categoría.</param>
    /// <param name="request">
    /// Nuevos datos de la categoría.
    /// </param>
    /// <returns>
    /// La categoría actualizada.
    /// </returns>
    /// <response code="200">
    /// Categoría actualizada correctamente.
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
    /// <response code="404">
    /// La categoría no existe.
    /// </response>
    /// <response code="409">
    /// Ya existe otra categoría con el mismo nombre.
    /// </response>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(CategoryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CategoryResponse>> Update(
        int id,
        UpdateCategoryRequest request)
    {
        try
        {
            var updatedCategory =
                await _categoryService.UpdateAsync(id, request);

            if (updatedCategory is null)
            {
                return NotFound(new
                {
                    message = "La categoría no existe."
                });
            }

            return Ok(updatedCategory);
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
    /// Elimina una categoría existente.
    /// </summary>
    /// <param name="id">Identificador de la categoría.</param>
    /// <response code="204">
    /// Categoría eliminada correctamente.
    /// </response>
    /// <response code="401">
    /// El usuario no está autenticado.
    /// </response>
    /// <response code="403">
    /// El usuario autenticado no tiene el rol Admin.
    /// </response>
    /// <response code="404">
    /// La categoría no existe.
    /// </response>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _categoryService.DeleteAsync(id);

        if (!deleted)
        {
            return NotFound(new
            {
                message = "La categoría no existe."
            });
        }

        return NoContent();
    }
}