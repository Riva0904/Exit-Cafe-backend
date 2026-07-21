using ExitCafe.Application.Features.Categories.Dtos;
using MediatR;

namespace ExitCafe.Application.Features.Categories.Queries.GetCategoryBySlug;

public record GetCategoryBySlugQuery(string Slug) : IRequest<CategoryDto>;
