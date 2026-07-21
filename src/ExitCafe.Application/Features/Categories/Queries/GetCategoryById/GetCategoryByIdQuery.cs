using ExitCafe.Application.Features.Categories.Dtos;
using MediatR;

namespace ExitCafe.Application.Features.Categories.Queries.GetCategoryById;

public record GetCategoryByIdQuery(Guid Id) : IRequest<CategoryDto>;
