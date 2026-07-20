using ExitCafe.Domain.Common;

namespace ExitCafe.Domain.Entities;

public class Product : BaseAuditableEntity
{
    public string Name { get; set; } = default!;
    public string Slug { get; set; } = default!;
    public string? ShortDescription { get; set; }
    public string? Description { get; set; }
    public string SKU { get; set; } = default!;

    public decimal Price { get; set; }
    public decimal? DiscountPrice { get; set; }

    public string? Ingredients { get; set; }
    public string? NutritionInfo { get; set; }

    public bool IsAvailable { get; set; } = true;
    public bool IsFeatured { get; set; }
    public bool IsBestSeller { get; set; }
    public bool IsNewArrival { get; set; }
    public bool IsTodaysSpecial { get; set; }

    public decimal AverageRating { get; set; }
    public int ReviewCount { get; set; }
    public int StockQuantity { get; set; }

    public Guid CategoryId { get; set; }
    public Category Category { get; set; } = default!;

    public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
