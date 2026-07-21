using ExitCafe.Application.Features.Products.Dtos;
using MediatR;

namespace ExitCafe.Application.Features.Products.Queries.GetFeaturedProducts;

public record GetFeaturedProductsQuery : IRequest<List<ProductListItemDto>>;
