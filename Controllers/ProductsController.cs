using API_Gestion_Inventario.DTOs.Products;
using API_Gestion_Inventario.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API_Gestion_Inventario.Controllers;

/// <summary>
/// Gestiona las operaciones de consulta y administración de productos.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    /// <summary>
    /// Obtiene todos los productos registrados.
    /// </summary>
    /// <returns>
    /// Lista de productos.
    /// </returns>
    /// <response code="200">
    /// Productos obtenidos correctamente.
    /// </response>
    /// <response code="401">
    /// El usuario no está autenticado.
    /// </response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ProductResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IEnumerable<ProductResponse>>> GetAll()
    {
        var products = await _productService.GetAllAsync();

        return Ok(products);
    }

    /// <summary>
    /// Obtiene un producto mediante su identificador.
    /// </summary>
    /// <param name="id">Identificador del producto.</param>
    /// <returns>
    /// Información del producto solicitado.
    /// </returns>
    /// <response code="200">
    /// Producto encontrado correctamente.
    /// </response>
    /// <response code="401">
    /// El usuario no está autenticado.
    /// </response>
    /// <response code="404">
    /// El producto no existe.
    /// </response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductResponse>> GetById(int id)
    {
        var product = await _productService.GetByIdAsync(id);

        if (product is null)
        {
            return NotFound(new
            {
                message = "El producto no existe."
            });
        }

        return Ok(product);
    }

    /// <summary>
    /// Crea un nuevo producto.
    /// </summary>
    /// <param name="request">
    /// Datos necesarios para crear el producto.
    /// </param>
    /// <returns>
    /// El producto creado.
    /// </returns>
    /// <response code="201">
    /// Producto creado correctamente.
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
    /// Ya existe un producto con el mismo nombre o la categoría indicada no es válida.
    /// </response>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ProductResponse>> Create(
        CreateProductRequest request)
    {
        try
        {
            var createdProduct =
                await _productService.CreateAsync(request);

            return CreatedAtAction(
                nameof(GetById),
                new { id = createdProduct.Id },
                createdProduct);
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
    /// Actualiza un producto existente.
    /// </summary>
    /// <param name="id">Identificador del producto.</param>
    /// <param name="request">
    /// Nuevos datos del producto.
    /// </param>
    /// <returns>
    /// El producto actualizado.
    /// </returns>
    /// <response code="200">
    /// Producto actualizado correctamente.
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
    /// El producto no existe.
    /// </response>
    /// <response code="409">
    /// Ya existe otro producto con el mismo nombre o la categoría indicada no es válida.
    /// </response>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ProductResponse>> Update(
        int id,
        UpdateProductRequest request)
    {
        try
        {
            var updatedProduct =
                await _productService.UpdateAsync(id, request);

            if (updatedProduct is null)
            {
                return NotFound(new
                {
                    message = "El producto no existe."
                });
            }

            return Ok(updatedProduct);
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
    /// Elimina un producto existente.
    /// </summary>
    /// <param name="id">Identificador del producto.</param>
    /// <response code="204">
    /// Producto eliminado correctamente.
    /// </response>
    /// <response code="401">
    /// El usuario no está autenticado.
    /// </response>
    /// <response code="403">
    /// El usuario autenticado no tiene el rol Admin.
    /// </response>
    /// <response code="404">
    /// El producto no existe.
    /// </response>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _productService.DeleteAsync(id);

        if (!deleted)
        {
            return NotFound(new
            {
                message = "El producto no existe."
            });
        }

        return NoContent();
    }
}