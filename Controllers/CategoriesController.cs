using API_Gestion_Inventario.DTOs.Categories;
using API_Gestion_Inventario.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API_Gestion_Inventario.Controllers;

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

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoryResponse>>> GetAll()
    {
        var categories = await _categoryService.GetAllAsync();

        return Ok(categories);
    }

    [HttpGet("{id:int}")]
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

    [HttpPost]
    [Authorize(Roles = "Admin")]
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

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
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

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
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
