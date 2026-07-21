using ExitCafe.Application.Features.Products.Dtos;
using MediatR;

namespace ExitCafe.Application.Features.Products.Queries.GetBestSellerProducts;

public record GetBestSellerProductsQuery : IRequest<List<ProductListItemDto>>;
