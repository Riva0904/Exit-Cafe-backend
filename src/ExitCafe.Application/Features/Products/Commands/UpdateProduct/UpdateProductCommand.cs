using ExitCafe.Application.Features.Products.Dtos;
using MediatR;

namespace ExitCafe.Application.Features.Products.Commands.UpdateProduct;

public record UpdateProductCommand(
    Guid Id, string Name, string? ShortDescription, string? Description,
    decimal Price, decimal? DiscountPrice, string? Ingredients, string? NutritionInfo,
    bool IsAvailable, bool IsFeatured, bool IsBestSeller, bool IsNewArrival, bool IsTodaysSpecial,
    int StockQuantity, Guid CategoryId) : IRequest<ProductDto>;
