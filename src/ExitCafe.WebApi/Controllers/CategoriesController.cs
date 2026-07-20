using ExitCafe.Application.Common.Models;
using ExitCafe.Application.DTOs.Categories;
using ExitCafe.Application.DTOs.Products;
using ExitCafe.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExitCafe.WebApi.Controllers;

[ApiController]
[Route("api/categories")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categoryService;
    private readonly IProductService _productService;

    public CategoriesController(ICategoryService categoryService, IProductService productService)
    {
        _categoryService = categoryService;
        _productService = productService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<CategoryDto>>>> GetAll([FromQuery] bool includeInactive, CancellationToken ct)
    {
        var result = await _categoryService.GetAllAsync(includeInactive, ct);
        return Ok(ApiResponse<List<CategoryDto>>.Ok(result));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<CategoryDto>>> GetById(Guid id, CancellationToken ct)
    {
        var result = await _categoryService.GetByIdAsync(id, ct);
        return Ok(ApiResponse<CategoryDto>.Ok(result));
    }

    [HttpGet("slug/{slug}")]
    public async Task<ActionResult<ApiResponse<CategoryDto>>> GetBySlug(string slug, CancellationToken ct)
    {
        var result = await _categoryService.GetBySlugAsync(slug, ct);
        return Ok(ApiResponse<CategoryDto>.Ok(result));
    }

    [HttpGet("{slug}/products")]
    public async Task<ActionResult<ApiResponse<PagedResult<ProductListItemDto>>>> GetProducts(
        string slug, [FromQuery] ProductQueryParams query, CancellationToken ct)
    {
        var category = await _categoryService.GetBySlugAsync(slug, ct);
        query.CategoryId = category.Id;
        var result = await _productService.GetAllAsync(query, ct);
        return Ok(ApiResponse<PagedResult<ProductListItemDto>>.Ok(result));
    }

    [HttpPost]
    [Authorize(Policy = "StaffAndAbove")]
    public async Task<ActionResult<ApiResponse<CategoryDto>>> Create(CreateCategoryRequest request, CancellationToken ct)
    {
        var result = await _categoryService.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, ApiResponse<CategoryDto>.Ok(result, "Category created."));
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = "StaffAndAbove")]
    public async Task<ActionResult<ApiResponse<CategoryDto>>> Update(Guid id, UpdateCategoryRequest request, CancellationToken ct)
    {
        var result = await _categoryService.UpdateAsync(id, request, ct);
        return Ok(ApiResponse<CategoryDto>.Ok(result, "Category updated."));
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _categoryService.DeleteAsync(id, ct);
        return NoContent();
    }
}
