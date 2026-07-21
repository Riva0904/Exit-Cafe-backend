using AutoMapper;
using ExitCafe.Application.Common.Interfaces;
using ExitCafe.Application.Features.Products.Dtos;
using ExitCafe.Domain.Entities;
using ExitCafe.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExitCafe.Application.Features.Products.Commands.UpdateProductImages;

public class UpdateProductImagesCommandHandler : IRequestHandler<UpdateProductImagesCommand, ProductDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public UpdateProductImagesCommandHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    private IQueryable<Product> BaseQuery() =>
        _uow.Products.Query().Include(p => p.Images).Include(p => p.Category);

    public async Task<ProductDto> Handle(UpdateProductImagesCommand request, CancellationToken ct)
    {
        var product = await BaseQuery().FirstOrDefaultAsync(p => p.Id == request.Id, ct)
            ?? throw new NotFoundException(nameof(Product), request.Id);

        foreach (var image in product.Images.ToList())
            _uow.ProductImages.Remove(image);

        for (var idx = 0; idx < request.ImageUrls.Count; idx++)
        {
            await _uow.ProductImages.AddAsync(new ProductImage
            {
                ProductId = product.Id,
                ImageUrl = request.ImageUrls[idx],
                IsPrimary = idx == 0,
                DisplayOrder = idx
            }, ct);
        }

        await _uow.SaveChangesAsync(ct);

        var updated = await BaseQuery().FirstAsync(p => p.Id == product.Id, ct);
        return _mapper.Map<ProductDto>(updated);
    }
}
