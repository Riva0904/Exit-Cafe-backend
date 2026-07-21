using AutoMapper;
using ExitCafe.Application.Common.Interfaces;
using ExitCafe.Application.Common.Utilities;
using ExitCafe.Application.Features.Products.Dtos;
using ExitCafe.Domain.Entities;
using ExitCafe.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExitCafe.Application.Features.Products.Commands.UpdateProduct;

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, ProductDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public UpdateProductCommandHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    private IQueryable<Product> BaseQuery() =>
        _uow.Products.Query().Include(p => p.Images).Include(p => p.Category);

    public async Task<ProductDto> Handle(UpdateProductCommand request, CancellationToken ct)
    {
        var product = await _uow.Products.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException(nameof(Product), request.Id);

        if (!await _uow.Categories.AnyAsync(c => c.Id == request.CategoryId, ct))
            throw new NotFoundException(nameof(Category), request.CategoryId);

        product.Name = request.Name;
        product.Slug = SlugHelper.GenerateSlug(request.Name);
        product.ShortDescription = request.ShortDescription;
        product.Description = request.Description;
        product.Price = request.Price;
        product.DiscountPrice = request.DiscountPrice;
        product.Ingredients = request.Ingredients;
        product.NutritionInfo = request.NutritionInfo;
        product.IsAvailable = request.IsAvailable;
        product.IsFeatured = request.IsFeatured;
        product.IsBestSeller = request.IsBestSeller;
        product.IsNewArrival = request.IsNewArrival;
        product.IsTodaysSpecial = request.IsTodaysSpecial;
        product.StockQuantity = request.StockQuantity;
        product.CategoryId = request.CategoryId;

        // No explicit Update(): product is already tracked from GetByIdAsync (a plain FindAsync with
        // no Include), so its Images navigation is the entity's default empty list, not the real
        // photos. Update() would mark that empty collection authoritative for this Cascade-configured
        // relationship and delete the real ProductImages rows on save.
        await _uow.SaveChangesAsync(ct);

        var updated = await BaseQuery().FirstAsync(p => p.Id == product.Id, ct);
        return _mapper.Map<ProductDto>(updated);
    }
}
