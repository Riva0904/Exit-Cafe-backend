using ExitCafe.Application.Features.Products.Dtos;
using MediatR;

namespace ExitCafe.Application.Features.Products.Commands.UpdateProductImages;

public record UpdateProductImagesCommand(Guid Id, List<string> ImageUrls) : IRequest<ProductDto>;
