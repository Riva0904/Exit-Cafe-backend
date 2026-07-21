using ExitCafe.Application.Features.Categories.Dtos;
using MediatR;

namespace ExitCafe.Application.Features.Categories.Commands.UpdateCategory;

public record UpdateCategoryCommand(Guid Id, string Name, string? Description, string? ImageUrl, int DisplayOrder, bool IsActive, Guid? ParentCategoryId) : IRequest<CategoryDto>;
