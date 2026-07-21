using AutoMapper;
using ExitCafe.Application.Common.Interfaces;
using ExitCafe.Application.Common.Utilities;
using ExitCafe.Application.Features.Categories.Dtos;
using ExitCafe.Domain.Entities;
using ExitCafe.Domain.Exceptions;
using MediatR;

namespace ExitCafe.Application.Features.Categories.Commands.CreateCategory;

public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, CategoryDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public CreateCategoryCommandHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<CategoryDto> Handle(CreateCategoryCommand request, CancellationToken ct)
    {
        var slug = SlugHelper.GenerateSlug(request.Name);
        if (await _uow.Categories.AnyAsync(c => c.Slug == slug, ct))
            throw new ConflictException($"Category with slug '{slug}' already exists.");

        var category = new Category
        {
            Name = request.Name,
            Slug = slug,
            Description = request.Description,
            ImageUrl = request.ImageUrl,
            DisplayOrder = request.DisplayOrder,
            ParentCategoryId = request.ParentCategoryId,
            IsActive = true
        };

        await _uow.Categories.AddAsync(category, ct);
        await _uow.SaveChangesAsync(ct);
        return _mapper.Map<CategoryDto>(category);
    }
}
