using ExitCafe.Application.Features.Products.Dtos;
using MediatR;

namespace ExitCafe.Application.Features.Products.Queries.GetNewArrivalProducts;

public record GetNewArrivalProductsQuery : IRequest<List<ProductListItemDto>>;
