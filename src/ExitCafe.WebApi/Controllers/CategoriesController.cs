using ExitCafe.Application.Common.Models;
using ExitCafe.Application.Features.Categories.Commands.CreateCategory;
using ExitCafe.Application.Features.Categories.Commands.DeleteCategory;
using ExitCafe.Application.Features.Categories.Commands.UpdateCategory;
using ExitCafe.Application.Features.Categories.Dtos;
using ExitCafe.Application.Features.Categories.Queries.GetCategories;
using ExitCafe.Application.Features.Categories.Queries.GetCategoryById;
using ExitCafe.Application.Features.Categories.Queries.GetCategoryBySlug;
using ExitCafe.Application.Features.Categories.Queries.GetCategoryProducts;
using ExitCafe.Application.Features.Products.Dtos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExitCafe.WebApi.Controllers;

[ApiController]
[Route("api/categories")]
public class CategoriesController : ControllerBase
{
    private readonly IMediator _mediator;

    public CategoriesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<CategoryDto>>>> GetAll([FromQuery] bool includeInactive, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetCategoriesQuery(includeInactive), ct);
        return Ok(ApiResponse<List<CategoryDto>>.Ok(result));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<CategoryDto>>> GetById(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetCategoryByIdQuery(id), ct);
        return Ok(ApiResponse<CategoryDto>.Ok(result));
    }

    [HttpGet("slug/{slug}")]
    public async Task<ActionResult<ApiResponse<CategoryDto>>> GetBySlug(string slug, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetCategoryBySlugQuery(slug), ct);
        return Ok(ApiResponse<CategoryDto>.Ok(result));
    }

    [HttpGet("{slug}/products")]
    public async Task<ActionResult<ApiResponse<PagedResult<ProductListItemDto>>>> GetProducts(
        string slug, [FromQuery] GetCategoryProductsQuery query, CancellationToken ct)
    {
        query.Slug = slug;
        var result = await _mediator.Send(query, ct);
        return Ok(ApiResponse<PagedResult<ProductListItemDto>>.Ok(result));
    }

    [HttpPost]
    [Authorize(Policy = "StaffAndAbove")]
    public async Task<ActionResult<ApiResponse<CategoryDto>>> Create(CreateCategoryCommand command, CancellationToken ct)
    {
        var result = await _mediator.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, ApiResponse<CategoryDto>.Ok(result, "Category created."));
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = "StaffAndAbove")]
    public async Task<ActionResult<ApiResponse<CategoryDto>>> Update(Guid id, UpdateCategoryCommand command, CancellationToken ct)
    {
        var result = await _mediator.Send(command with { Id = id }, ct);
        return Ok(ApiResponse<CategoryDto>.Ok(result, "Category updated."));
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _mediator.Send(new DeleteCategoryCommand(id), ct);
        return NoContent();
    }
}
