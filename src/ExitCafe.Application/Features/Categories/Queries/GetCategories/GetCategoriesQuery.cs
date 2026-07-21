using ExitCafe.Application.Features.Categories.Dtos;
using MediatR;

namespace ExitCafe.Application.Features.Categories.Queries.GetCategories;

public record GetCategoriesQuery(bool IncludeInactive) : IRequest<List<CategoryDto>>;
