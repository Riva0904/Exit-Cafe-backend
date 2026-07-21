using ExitCafe.Application.Features.Products.Dtos;
using MediatR;

namespace ExitCafe.Application.Features.Products.Queries.GetTodaysSpecialProducts;

public record GetTodaysSpecialProductsQuery : IRequest<List<ProductListItemDto>>;
