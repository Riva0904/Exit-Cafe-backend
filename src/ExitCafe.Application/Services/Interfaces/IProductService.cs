using ExitCafe.Application.Common.Models;
using ExitCafe.Application.DTOs.Products;

namespace ExitCafe.Application.Services.Interfaces;

public interface IProductService
{
    Task<PagedResult<ProductListItemDto>> GetAllAsync(ProductQueryParams query, CancellationToken ct = default);
    Task<ProductDto> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<ProductDto> GetBySlugAsync(string slug, CancellationToken ct = default);
    Task<List<ProductListItemDto>> GetFeaturedAsync(CancellationToken ct = default);
    Task<List<ProductListItemDto>> GetBestSellersAsync(CancellationToken ct = default);
    Task<List<ProductListItemDto>> GetNewArrivalsAsync(CancellationToken ct = default);
    Task<List<ProductListItemDto>> GetTodaysSpecialAsync(CancellationToken ct = default);
    Task<ProductDto> CreateAsync(CreateProductRequest request, CancellationToken ct = default);
    Task<ProductDto> UpdateAsync(Guid id, UpdateProductRequest request, CancellationToken ct = default);
    Task<ProductDto> UpdateImagesAsync(Guid id, UpdateProductImagesRequest request, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}
