using ExitCafe.Domain.Entities;

namespace ExitCafe.Application.Features.Products;

public static class ProductQueryBuilder
{
    public static IQueryable<Product> ApplyFilters(
        IQueryable<Product> q,
        Guid? categoryId,
        string? searchTerm,
        decimal? minPrice,
        decimal? maxPrice,
        bool? isFeatured,
        bool? isBestSeller,
        bool? isNewArrival,
        bool? isTodaysSpecial,
        bool? isAvailable)
    {
        if (categoryId.HasValue) q = q.Where(p => p.CategoryId == categoryId);
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.ToLower();
            q = q.Where(p => p.Name.ToLower().Contains(term) || (p.Description != null && p.Description.ToLower().Contains(term)));
        }
        if (minPrice.HasValue) q = q.Where(p => p.Price >= minPrice);
        if (maxPrice.HasValue) q = q.Where(p => p.Price <= maxPrice);
        if (isFeatured.HasValue) q = q.Where(p => p.IsFeatured == isFeatured);
        if (isBestSeller.HasValue) q = q.Where(p => p.IsBestSeller == isBestSeller);
        if (isNewArrival.HasValue) q = q.Where(p => p.IsNewArrival == isNewArrival);
        if (isTodaysSpecial.HasValue) q = q.Where(p => p.IsTodaysSpecial == isTodaysSpecial);
        if (isAvailable.HasValue) q = q.Where(p => p.IsAvailable == isAvailable);

        return q;
    }

    public static IQueryable<Product> ApplySort(IQueryable<Product> q, string? sortBy, bool sortDescending)
    {
        return sortBy?.ToLower() switch
        {
            "price" => sortDescending ? q.OrderByDescending(p => p.Price) : q.OrderBy(p => p.Price),
            "rating" => sortDescending ? q.OrderByDescending(p => p.AverageRating) : q.OrderBy(p => p.AverageRating),
            "name" => sortDescending ? q.OrderByDescending(p => p.Name) : q.OrderBy(p => p.Name),
            _ => q.OrderByDescending(p => p.CreatedAt)
        };
    }
}
