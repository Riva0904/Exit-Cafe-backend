namespace ExitCafe.Application.Features.Products.Dtos;

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
