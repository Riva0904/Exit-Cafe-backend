namespace ExitCafe.Application.DTOs.Products;

public class ProductImageDto
{
    public Guid Id { get; set; }
    public string ImageUrl { get; set; } = default!;
    public bool IsPrimary { get; set; }
    public int DisplayOrder { get; set; }
}

public class ProductListItemDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string Slug { get; set; } = default!;
    public decimal Price { get; set; }
    public decimal? DiscountPrice { get; set; }
    public string? PrimaryImageUrl { get; set; }
    public bool IsAvailable { get; set; }
    public bool IsFeatured { get; set; }
    public bool IsBestSeller { get; set; }
    public bool IsNewArrival { get; set; }
    public decimal AverageRating { get; set; }
    public int ReviewCount { get; set; }
    public string CategoryName { get; set; } = default!;
}

public class ProductDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string Slug { get; set; } = default!;
    public string? ShortDescription { get; set; }
    public string? Description { get; set; }
    public string SKU { get; set; } = default!;
    public decimal Price { get; set; }
    public decimal? DiscountPrice { get; set; }
    public string? Ingredients { get; set; }
    public string? NutritionInfo { get; set; }
    public bool IsAvailable { get; set; }
    public bool IsFeatured { get; set; }
    public bool IsBestSeller { get; set; }
    public bool IsNewArrival { get; set; }
    public bool IsTodaysSpecial { get; set; }
    public decimal AverageRating { get; set; }
    public int ReviewCount { get; set; }
    public int StockQuantity { get; set; }
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = default!;
    public List<ProductImageDto> Images { get; set; } = new();
    public List<ProductListItemDto> RelatedProducts { get; set; } = new();
}

public record CreateProductRequest(
    string Name, string? ShortDescription, string? Description, string SKU,
    decimal Price, decimal? DiscountPrice, string? Ingredients, string? NutritionInfo,
    bool IsAvailable, bool IsFeatured, bool IsBestSeller, bool IsNewArrival, bool IsTodaysSpecial,
    int StockQuantity, Guid CategoryId, List<string> ImageUrls);

public record UpdateProductRequest(
    string Name, string? ShortDescription, string? Description,
    decimal Price, decimal? DiscountPrice, string? Ingredients, string? NutritionInfo,
    bool IsAvailable, bool IsFeatured, bool IsBestSeller, bool IsNewArrival, bool IsTodaysSpecial,
    int StockQuantity, Guid CategoryId);

public class ProductQueryParams
{
    public Guid? CategoryId { get; set; }
    public string? SearchTerm { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public bool? IsFeatured { get; set; }
    public bool? IsBestSeller { get; set; }
    public bool? IsNewArrival { get; set; }
    public bool? IsTodaysSpecial { get; set; }
    public bool? IsAvailable { get; set; }
    public string? SortBy { get; set; }
    public bool SortDescending { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
