using ExitCafe.Application.Features.Products.Dtos;
using MediatR;

namespace ExitCafe.Application.Features.Products.Commands.CreateProduct;

public record CreateProductCommand(
    string Name, string? ShortDescription, string? Description, string SKU,
    decimal Price, decimal? DiscountPrice, string? Ingredients, string? NutritionInfo,
    bool IsAvailable, bool IsFeatured, bool IsBestSeller, bool IsNewArrival, bool IsTodaysSpecial,
    int StockQuantity, Guid CategoryId, List<string> ImageUrls) : IRequest<ProductDto>;
