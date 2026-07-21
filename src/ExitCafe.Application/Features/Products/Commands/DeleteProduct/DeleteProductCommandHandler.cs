using ExitCafe.Application.Common.Interfaces;
using ExitCafe.Domain.Entities;
using ExitCafe.Domain.Exceptions;
using MediatR;

namespace ExitCafe.Application.Features.Products.Commands.DeleteProduct;

public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand>
{
    private readonly IUnitOfWork _uow;

    public DeleteProductCommandHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task Handle(DeleteProductCommand request, CancellationToken ct)
    {
        var product = await _uow.Products.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException(nameof(Product), request.Id);

        product.IsDeleted = true;
        product.IsAvailable = false;
        _uow.Products.Update(product);
        await _uow.SaveChangesAsync(ct);
    }
}
