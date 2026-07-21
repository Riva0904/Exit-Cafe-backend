using AutoMapper;
using ExitCafe.Application.Common.Interfaces;
using ExitCafe.Application.Common.Utilities;
using ExitCafe.Application.Features.Categories.Dtos;
using ExitCafe.Domain.Entities;
using ExitCafe.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExitCafe.Application.Features.Categories.Commands.UpdateCategory;

public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, CategoryDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public UpdateCategoryCommandHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<CategoryDto> Handle(UpdateCategoryCommand request, CancellationToken ct)
    {
        var category = await _uow.Categories.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException(nameof(Category), request.Id);

        category.Name = request.Name;
        category.Slug = SlugHelper.GenerateSlug(request.Name);
        category.Description = request.Description;
        category.ImageUrl = request.ImageUrl;
        category.DisplayOrder = request.DisplayOrder;
        category.IsActive = request.IsActive;
        category.ParentCategoryId = request.ParentCategoryId;

        _uow.Categories.Update(category);
        await _uow.SaveChangesAsync(ct);

        var updated = await _uow.Categories.Query().Include(c => c.Products).FirstAsync(c => c.Id == category.Id, ct);
        return _mapper.Map<CategoryDto>(updated);
    }
}
