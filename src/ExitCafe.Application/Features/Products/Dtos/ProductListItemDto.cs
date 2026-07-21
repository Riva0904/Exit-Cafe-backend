namespace ExitCafe.Application.Features.Products.Dtos;

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
    public bool IsTodaysSpecial { get; set; }
    public decimal AverageRating { get; set; }
    public int ReviewCount { get; set; }
    public int StockQuantity { get; set; }
    public string CategoryName { get; set; } = default!;
}
