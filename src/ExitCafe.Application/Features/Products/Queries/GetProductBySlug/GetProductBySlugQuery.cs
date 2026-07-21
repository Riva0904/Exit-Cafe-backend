using ExitCafe.Application.Features.Products.Dtos;
using MediatR;

namespace ExitCafe.Application.Features.Products.Queries.GetProductBySlug;

public record GetProductBySlugQuery(string Slug) : IRequest<ProductDto>;
