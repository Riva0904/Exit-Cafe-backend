using ExitCafe.Application.Common.Models;
using ExitCafe.Application.DTOs.Products;
using ExitCafe.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExitCafe.WebApi.Controllers;

[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<ProductListItemDto>>>> GetAll([FromQuery] ProductQueryParams query, CancellationToken ct)
    {
        var result = await _productService.GetAllAsync(query, ct);
        return Ok(ApiResponse<PagedResult<ProductListItemDto>>.Ok(result));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<ProductDto>>> GetById(Guid id, CancellationToken ct)
    {
        var result = await _productService.GetByIdAsync(id, ct);
        return Ok(ApiResponse<ProductDto>.Ok(result));
    }

    [HttpGet("slug/{slug}")]
    public async Task<ActionResult<ApiResponse<ProductDto>>> GetBySlug(string slug, CancellationToken ct)
    {
        var result = await _productService.GetBySlugAsync(slug, ct);
        return Ok(ApiResponse<ProductDto>.Ok(result));
    }

    [HttpGet("featured")]
    public async Task<ActionResult<ApiResponse<List<ProductListItemDto>>>> GetFeatured(CancellationToken ct) =>
        Ok(ApiResponse<List<ProductListItemDto>>.Ok(await _productService.GetFeaturedAsync(ct)));

    [HttpGet("best-sellers")]
    public async Task<ActionResult<ApiResponse<List<ProductListItemDto>>>> GetBestSellers(CancellationToken ct) =>
        Ok(ApiResponse<List<ProductListItemDto>>.Ok(await _productService.GetBestSellersAsync(ct)));

    [HttpGet("new-arrivals")]
    public async Task<ActionResult<ApiResponse<List<ProductListItemDto>>>> GetNewArrivals(CancellationToken ct) =>
        Ok(ApiResponse<List<ProductListItemDto>>.Ok(await _productService.GetNewArrivalsAsync(ct)));

    [HttpGet("todays-special")]
    public async Task<ActionResult<ApiResponse<List<ProductListItemDto>>>> GetTodaysSpecial(CancellationToken ct) =>
        Ok(ApiResponse<List<ProductListItemDto>>.Ok(await _productService.GetTodaysSpecialAsync(ct)));

    [HttpPost]
    [Authorize(Policy = "StaffAndAbove")]
    public async Task<ActionResult<ApiResponse<ProductDto>>> Create(CreateProductRequest request, CancellationToken ct)
    {
        var result = await _productService.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, ApiResponse<ProductDto>.Ok(result, "Product created."));
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = "StaffAndAbove")]
    public async Task<ActionResult<ApiResponse<ProductDto>>> Update(Guid id, UpdateProductRequest request, CancellationToken ct)
    {
        var result = await _productService.UpdateAsync(id, request, ct);
        return Ok(ApiResponse<ProductDto>.Ok(result, "Product updated."));
    }

    [HttpPut("{id:guid}/images")]
    [Authorize(Policy = "StaffAndAbove")]
    public async Task<ActionResult<ApiResponse<ProductDto>>> UpdateImages(Guid id, UpdateProductImagesRequest request, CancellationToken ct)
    {
        var result = await _productService.UpdateImagesAsync(id, request, ct);
        return Ok(ApiResponse<ProductDto>.Ok(result, "Product images updated."));
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _productService.DeleteAsync(id, ct);
        return NoContent();
    }
}
