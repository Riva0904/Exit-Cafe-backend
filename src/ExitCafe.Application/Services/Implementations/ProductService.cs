using AutoMapper;
using ExitCafe.Application.Common.Interfaces;
using ExitCafe.Application.Common.Models;
using ExitCafe.Application.Common.Utilities;
using ExitCafe.Application.DTOs.Products;
using ExitCafe.Application.Services.Interfaces;
using ExitCafe.Domain.Entities;
using ExitCafe.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace ExitCafe.Application.Services.Implementations;

public class ProductService : IProductService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public ProductService(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    private IQueryable<Product> BaseQuery() =>
        _uow.Products.Query().Include(p => p.Images).Include(p => p.Category);

    public async Task<PagedResult<ProductListItemDto>> GetAllAsync(ProductQueryParams query, CancellationToken ct = default)
    {
        var q = BaseQuery();

        if (query.CategoryId.HasValue) q = q.Where(p => p.CategoryId == query.CategoryId);
        if (!string.IsNullOrWhiteSpace(query.SearchTerm))
            q = q.Where(p => p.Name.Contains(query.SearchTerm) || p.Description!.Contains(query.SearchTerm));
        if (query.MinPrice.HasValue) q = q.Where(p => p.Price >= query.MinPrice);
        if (query.MaxPrice.HasValue) q = q.Where(p => p.Price <= query.MaxPrice);
        if (query.IsFeatured.HasValue) q = q.Where(p => p.IsFeatured == query.IsFeatured);
        if (query.IsBestSeller.HasValue) q = q.Where(p => p.IsBestSeller == query.IsBestSeller);
        if (query.IsNewArrival.HasValue) q = q.Where(p => p.IsNewArrival == query.IsNewArrival);
        if (query.IsTodaysSpecial.HasValue) q = q.Where(p => p.IsTodaysSpecial == query.IsTodaysSpecial);
        if (query.IsAvailable.HasValue) q = q.Where(p => p.IsAvailable == query.IsAvailable);

        q = query.SortBy?.ToLower() switch
        {
            "price" => query.SortDescending ? q.OrderByDescending(p => p.Price) : q.OrderBy(p => p.Price),
            "rating" => query.SortDescending ? q.OrderByDescending(p => p.AverageRating) : q.OrderBy(p => p.AverageRating),
            "name" => query.SortDescending ? q.OrderByDescending(p => p.Name) : q.OrderBy(p => p.Name),
            _ => q.OrderByDescending(p => p.CreatedAt)
        };

        var totalCount = await q.CountAsync(ct);
        var items = await q.Skip((query.PageNumber - 1) * query.PageSize).Take(query.PageSize).ToListAsync(ct);

        return new PagedResult<ProductListItemDto>
        {
            Items = _mapper.Map<List<ProductListItemDto>>(items),
            PageNumber = query.PageNumber,
            PageSize = query.PageSize,
            TotalCount = totalCount
        };
    }

    public async Task<ProductDto> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var product = await BaseQuery().FirstOrDefaultAsync(p => p.Id == id, ct)
            ?? throw new NotFoundException(nameof(Product), id);
        return await MapWithRelatedAsync(product, ct);
    }

    public async Task<ProductDto> GetBySlugAsync(string slug, CancellationToken ct = default)
    {
        var product = await BaseQuery().FirstOrDefaultAsync(p => p.Slug == slug, ct)
            ?? throw new NotFoundException(nameof(Product), slug);
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

    public async Task<List<ProductListItemDto>> GetFeaturedAsync(CancellationToken ct = default) =>
        _mapper.Map<List<ProductListItemDto>>(await BaseQuery().Where(p => p.IsFeatured && p.IsAvailable).Take(8).ToListAsync(ct));

    public async Task<List<ProductListItemDto>> GetBestSellersAsync(CancellationToken ct = default) =>
        _mapper.Map<List<ProductListItemDto>>(await BaseQuery().Where(p => p.IsBestSeller && p.IsAvailable).Take(8).ToListAsync(ct));

    public async Task<List<ProductListItemDto>> GetNewArrivalsAsync(CancellationToken ct = default) =>
        _mapper.Map<List<ProductListItemDto>>(await BaseQuery().Where(p => p.IsNewArrival && p.IsAvailable).Take(8).ToListAsync(ct));

    public async Task<List<ProductListItemDto>> GetTodaysSpecialAsync(CancellationToken ct = default) =>
        _mapper.Map<List<ProductListItemDto>>(await BaseQuery().Where(p => p.IsTodaysSpecial && p.IsAvailable).Take(8).ToListAsync(ct));

    public async Task<ProductDto> CreateAsync(CreateProductRequest request, CancellationToken ct = default)
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

    public async Task<ProductDto> UpdateAsync(Guid id, UpdateProductRequest request, CancellationToken ct = default)
    {
        var product = await _uow.Products.GetByIdAsync(id, ct)
            ?? throw new NotFoundException(nameof(Product), id);

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

        _uow.Products.Update(product);
        await _uow.SaveChangesAsync(ct);

        var updated = await BaseQuery().FirstAsync(p => p.Id == product.Id, ct);
        return _mapper.Map<ProductDto>(updated);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var product = await _uow.Products.GetByIdAsync(id, ct)
            ?? throw new NotFoundException(nameof(Product), id);

        product.IsDeleted = true;
        product.IsAvailable = false;
        _uow.Products.Update(product);
        await _uow.SaveChangesAsync(ct);
    }
}
