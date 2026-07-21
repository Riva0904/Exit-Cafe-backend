using ExitCafe.Application.Features.Categories.Dtos;
using MediatR;

namespace ExitCafe.Application.Features.Categories.Commands.CreateCategory;

public record CreateCategoryCommand(string Name, string? Description, string? ImageUrl, int DisplayOrder, Guid? ParentCategoryId) : IRequest<CategoryDto>;
