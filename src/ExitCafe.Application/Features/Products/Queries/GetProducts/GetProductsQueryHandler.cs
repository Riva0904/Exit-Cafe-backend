using AutoMapper;
using ExitCafe.Application.Common.Interfaces;
using ExitCafe.Application.Common.Models;
using ExitCafe.Application.Features.Products.Dtos;
using ExitCafe.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExitCafe.Application.Features.Products.Queries.GetProducts;

public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, PagedResult<ProductListItemDto>>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public GetProductsQueryHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<PagedResult<ProductListItemDto>> Handle(GetProductsQuery request, CancellationToken ct)
    {
        IQueryable<Product> q = _uow.Products.Query().Include(p => p.Images).Include(p => p.Category);

        q = ProductQueryBuilder.ApplyFilters(
            q, request.CategoryId, request.SearchTerm, request.MinPrice, request.MaxPrice,
            request.IsFeatured, request.IsBestSeller, request.IsNewArrival, request.IsTodaysSpecial, request.IsAvailable);

        q = ProductQueryBuilder.ApplySort(q, request.SortBy, request.SortDescending);

        var totalCount = await q.CountAsync(ct);
        var items = await q.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToListAsync(ct);

        return new PagedResult<ProductListItemDto>
        {
            Items = _mapper.Map<List<ProductListItemDto>>(items),
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }
}
