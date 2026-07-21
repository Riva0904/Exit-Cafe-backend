using AutoMapper;
using ExitCafe.Application.Common.Interfaces;
using ExitCafe.Application.Features.Products.Dtos;
using ExitCafe.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExitCafe.Application.Features.Products.Queries.GetTodaysSpecialProducts;

public class GetTodaysSpecialProductsQueryHandler : IRequestHandler<GetTodaysSpecialProductsQuery, List<ProductListItemDto>>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public GetTodaysSpecialProductsQueryHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    private IQueryable<Product> BaseQuery() =>
        _uow.Products.Query().Include(p => p.Images).Include(p => p.Category);

    public async Task<List<ProductListItemDto>> Handle(GetTodaysSpecialProductsQuery request, CancellationToken ct) =>
        _mapper.Map<List<ProductListItemDto>>(await BaseQuery().Where(p => p.IsTodaysSpecial && p.IsAvailable).Take(8).ToListAsync(ct));
}
