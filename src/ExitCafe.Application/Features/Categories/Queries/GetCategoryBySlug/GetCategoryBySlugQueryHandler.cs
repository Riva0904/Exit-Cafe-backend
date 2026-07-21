using AutoMapper;
using ExitCafe.Application.Common.Interfaces;
using ExitCafe.Application.Features.Categories.Dtos;
using ExitCafe.Domain.Entities;
using ExitCafe.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExitCafe.Application.Features.Categories.Queries.GetCategoryBySlug;

public class GetCategoryBySlugQueryHandler : IRequestHandler<GetCategoryBySlugQuery, CategoryDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public GetCategoryBySlugQueryHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<CategoryDto> Handle(GetCategoryBySlugQuery request, CancellationToken ct)
    {
        var category = await _uow.Categories.Query().Include(c => c.Products)
            .FirstOrDefaultAsync(c => c.Slug == request.Slug, ct)
            ?? throw new NotFoundException(nameof(Category), request.Slug);
        return _mapper.Map<CategoryDto>(category);
    }
}
