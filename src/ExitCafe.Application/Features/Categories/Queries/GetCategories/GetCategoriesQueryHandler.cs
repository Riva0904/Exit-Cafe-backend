using AutoMapper;
using ExitCafe.Application.Common.Interfaces;
using ExitCafe.Application.Features.Categories.Dtos;
using ExitCafe.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExitCafe.Application.Features.Categories.Queries.GetCategories;

public class GetCategoriesQueryHandler : IRequestHandler<GetCategoriesQuery, List<CategoryDto>>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public GetCategoriesQueryHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<List<CategoryDto>> Handle(GetCategoriesQuery request, CancellationToken ct)
    {
        IQueryable<Category> query = _uow.Categories.Query().Include(c => c.Products);
        if (!request.IncludeInactive)
            query = query.Where(c => c.IsActive);

        var categories = await query.OrderBy(c => c.DisplayOrder).ToListAsync(ct);
        return _mapper.Map<List<CategoryDto>>(categories);
    }
}
