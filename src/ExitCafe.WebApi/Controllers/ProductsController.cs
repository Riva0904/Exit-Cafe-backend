using ExitCafe.Application.Common.Models;
using ExitCafe.Application.Features.Products.Commands.CreateProduct;
using ExitCafe.Application.Features.Products.Commands.DeleteProduct;
using ExitCafe.Application.Features.Products.Commands.UpdateProduct;
using ExitCafe.Application.Features.Products.Commands.UpdateProductImages;
using ExitCafe.Application.Features.Products.Dtos;
using ExitCafe.Application.Features.Products.Queries.GetBestSellerProducts;
using ExitCafe.Application.Features.Products.Queries.GetFeaturedProducts;
using ExitCafe.Application.Features.Products.Queries.GetNewArrivalProducts;
using ExitCafe.Application.Features.Products.Queries.GetProductById;
using ExitCafe.Application.Features.Products.Queries.GetProductBySlug;
using ExitCafe.Application.Features.Products.Queries.GetProducts;
using ExitCafe.Application.Features.Products.Queries.GetTodaysSpecialProducts;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExitCafe.WebApi.Controllers;

[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<ProductListItemDto>>>> GetAll([FromQuery] GetProductsQuery query, CancellationToken ct)
    {
        var result = await _mediator.Send(query, ct);
        return Ok(ApiResponse<PagedResult<ProductListItemDto>>.Ok(result));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<ProductDto>>> GetById(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetProductByIdQuery(id), ct);
        return Ok(ApiResponse<ProductDto>.Ok(result));
    }

    [HttpGet("slug/{slug}")]
    public async Task<ActionResult<ApiResponse<ProductDto>>> GetBySlug(string slug, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetProductBySlugQuery(slug), ct);
        return Ok(ApiResponse<ProductDto>.Ok(result));
    }

    [HttpGet("featured")]
    public async Task<ActionResult<ApiResponse<List<ProductListItemDto>>>> GetFeatured(CancellationToken ct) =>
        Ok(ApiResponse<List<ProductListItemDto>>.Ok(await _mediator.Send(new GetFeaturedProductsQuery(), ct)));

    [HttpGet("best-sellers")]
    public async Task<ActionResult<ApiResponse<List<ProductListItemDto>>>> GetBestSellers(CancellationToken ct) =>
        Ok(ApiResponse<List<ProductListItemDto>>.Ok(await _mediator.Send(new GetBestSellerProductsQuery(), ct)));

    [HttpGet("new-arrivals")]
    public async Task<ActionResult<ApiResponse<List<ProductListItemDto>>>> GetNewArrivals(CancellationToken ct) =>
        Ok(ApiResponse<List<ProductListItemDto>>.Ok(await _mediator.Send(new GetNewArrivalProductsQuery(), ct)));

    [HttpGet("todays-special")]
    public async Task<ActionResult<ApiResponse<List<ProductListItemDto>>>> GetTodaysSpecial(CancellationToken ct) =>
        Ok(ApiResponse<List<ProductListItemDto>>.Ok(await _mediator.Send(new GetTodaysSpecialProductsQuery(), ct)));

    [HttpPost]
    [Authorize(Policy = "StaffAndAbove")]
    public async Task<ActionResult<ApiResponse<ProductDto>>> Create(CreateProductCommand command, CancellationToken ct)
    {
        var result = await _mediator.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, ApiResponse<ProductDto>.Ok(result, "Product created."));
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = "StaffAndAbove")]
    public async Task<ActionResult<ApiResponse<ProductDto>>> Update(Guid id, UpdateProductCommand command, CancellationToken ct)
    {
        var result = await _mediator.Send(command with { Id = id }, ct);
        return Ok(ApiResponse<ProductDto>.Ok(result, "Product updated."));
    }

    [HttpPut("{id:guid}/images")]
    [Authorize(Policy = "StaffAndAbove")]
    public async Task<ActionResult<ApiResponse<ProductDto>>> UpdateImages(Guid id, UpdateProductImagesCommand command, CancellationToken ct)
    {
        var result = await _mediator.Send(command with { Id = id }, ct);
        return Ok(ApiResponse<ProductDto>.Ok(result, "Product images updated."));
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _mediator.Send(new DeleteProductCommand(id), ct);
        return NoContent();
    }
}
