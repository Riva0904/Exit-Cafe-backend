using ExitCafe.Application.Common.Models;
using ExitCafe.Application.Features.Products.Dtos;
using MediatR;

namespace ExitCafe.Application.Features.Products.Queries.GetProducts;

public class GetProductsQuery : PaginationParams, IRequest<PagedResult<ProductListItemDto>>
{
    public Guid? CategoryId { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public bool? IsFeatured { get; set; }
    public bool? IsBestSeller { get; set; }
    public bool? IsNewArrival { get; set; }
    public bool? IsTodaysSpecial { get; set; }
    public bool? IsAvailable { get; set; }
}
