using AutoMapper;
using ExitCafe.Application.Common.Interfaces;
using ExitCafe.Application.Features.Products.Dtos;
using ExitCafe.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExitCafe.Application.Features.Products.Queries.GetFeaturedProducts;

public class GetFeaturedProductsQueryHandler : IRequestHandler<GetFeaturedProductsQuery, List<ProductListItemDto>>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public GetFeaturedProductsQueryHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    private IQueryable<Product> BaseQuery() =>
        _uow.Products.Query().Include(p => p.Images).Include(p => p.Category);

    public async Task<List<ProductListItemDto>> Handle(GetFeaturedProductsQuery request, CancellationToken ct) =>
        _mapper.Map<List<ProductListItemDto>>(await BaseQuery().Where(p => p.IsFeatured && p.IsAvailable).Take(8).ToListAsync(ct));
}
