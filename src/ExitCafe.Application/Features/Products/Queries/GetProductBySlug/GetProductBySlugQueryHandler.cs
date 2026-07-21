using AutoMapper;
using ExitCafe.Application.Common.Interfaces;
using ExitCafe.Application.Features.Products.Dtos;
using ExitCafe.Domain.Entities;
using ExitCafe.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExitCafe.Application.Features.Products.Queries.GetProductBySlug;

public class GetProductBySlugQueryHandler : IRequestHandler<GetProductBySlugQuery, ProductDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public GetProductBySlugQueryHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    private IQueryable<Product> BaseQuery() =>
        _uow.Products.Query().Include(p => p.Images).Include(p => p.Category);

    public async Task<ProductDto> Handle(GetProductBySlugQuery request, CancellationToken ct)
    {
        var product = await BaseQuery().FirstOrDefaultAsync(p => p.Slug == request.Slug, ct)
            ?? throw new NotFoundException(nameof(Product), request.Slug);
        return await MapWithRelatedAsync(product, ct);
    }

    private async Task<ProductDto> MapWithRelatedAsync(Product product, CancellationToken ct)
    {
        var dto = _mapper.Map<ProductDto>(product);
        var related = await BaseQuery()
            .Where(p => p.CategoryId == product.CategoryId && p.Id != product.Id)
            .Take(4).ToListAsync(ct);
        dto.RelatedProducts = _mapper.Map<List<ProductListItemDto>>(related);
        return dto;
    }
}
