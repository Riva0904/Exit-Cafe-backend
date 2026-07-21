using ExitCafe.Application.Common.Models;
using ExitCafe.Application.Features.Products.Dtos;
using MediatR;

namespace ExitCafe.Application.Features.Categories.Queries.GetCategoryProducts;

public class GetCategoryProductsQuery : PaginationParams, IRequest<PagedResult<ProductListItemDto>>
{
    // Nullable so [FromQuery] binding doesn't treat it as a required query-string param — the
    // controller always overwrites it from the route segment before dispatching.
    public string? Slug { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public bool? IsFeatured { get; set; }
    public bool? IsBestSeller { get; set; }
    public bool? IsNewArrival { get; set; }
    public bool? IsTodaysSpecial { get; set; }
    public bool? IsAvailable { get; set; }
}
