using AutoMapper;
using ExitCafe.Application.Common.Interfaces;
using ExitCafe.Application.Common.Utilities;
using ExitCafe.Application.Features.Products.Dtos;
using ExitCafe.Domain.Entities;
using ExitCafe.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExitCafe.Application.Features.Products.Commands.CreateProduct;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, ProductDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public CreateProductCommandHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    private IQueryable<Product> BaseQuery() =>
        _uow.Products.Query().Include(p => p.Images).Include(p => p.Category);

    public async Task<ProductDto> Handle(CreateProductCommand request, CancellationToken ct)
    {
        if (!await _uow.Categories.AnyAsync(c => c.Id == request.CategoryId, ct))
            throw new NotFoundException(nameof(Category), request.CategoryId);

        var slug = SlugHelper.GenerateSlug(request.Name);
        if (await _uow.Products.AnyAsync(p => p.Slug == slug, ct))
            slug = $"{slug}-{Guid.NewGuid().ToString()[..6]}";

        var product = new Product
        {
            Name = request.Name,
            Slug = slug,
            ShortDescription = request.ShortDescription,
            Description = request.Description,
            SKU = request.SKU,
            Price = request.Price,
            DiscountPrice = request.DiscountPrice,
            Ingredients = request.Ingredients,
            NutritionInfo = request.NutritionInfo,
            IsAvailable = request.IsAvailable,
            IsFeatured = request.IsFeatured,
            IsBestSeller = request.IsBestSeller,
            IsNewArrival = request.IsNewArrival,
            IsTodaysSpecial = request.IsTodaysSpecial,
            StockQuantity = request.StockQuantity,
            CategoryId = request.CategoryId,
            Images = request.ImageUrls.Select((url, idx) => new ProductImage
            {
                ImageUrl = url,
                IsPrimary = idx == 0,
                DisplayOrder = idx
            }).ToList()
        };

        await _uow.Products.AddAsync(product, ct);
        await _uow.SaveChangesAsync(ct);

        var created = await BaseQuery().FirstAsync(p => p.Id == product.Id, ct);
        return _mapper.Map<ProductDto>(created);
    }
}
