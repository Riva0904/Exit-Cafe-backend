using ExitCafe.Application.Common.Interfaces;
using ExitCafe.Domain.Entities;
using ExitCafe.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExitCafe.Application.Features.Categories.Commands.DeleteCategory;

public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand>
{
    private readonly IUnitOfWork _uow;
    private readonly IAuditLogService _auditLog;

    public DeleteCategoryCommandHandler(IUnitOfWork uow, IAuditLogService auditLog)
    {
        _uow = uow;
        _auditLog = auditLog;
    }

    public async Task Handle(DeleteCategoryCommand request, CancellationToken ct)
    {
        var category = await _uow.Categories.Query().Include(c => c.Products)
            .FirstOrDefaultAsync(c => c.Id == request.Id, ct)
            ?? throw new NotFoundException(nameof(Category), request.Id);

        // A soft-deleted category is excluded by its own query filter, which silently drops any
        // product still assigned to it out of Include()-based joins (count queries still see them,
        // list queries don't — a confusing, hard-to-spot data gap). Deactivate instead of blocking
        // outright would still hide products from customers, so require the category to be emptied
        // (or its products reassigned) first.
        if (category.Products.Any(p => !p.IsDeleted))
            throw new ConflictException(
                $"Cannot delete '{category.Name}': it still has {category.Products.Count(p => !p.IsDeleted)} product(s). Reassign or remove them first.");

        category.IsDeleted = true;
        category.IsActive = false;
        _uow.Categories.Update(category);
        await _uow.SaveChangesAsync(ct);
        await _auditLog.LogAsync("CategoryDeleted", nameof(Category), category.Id.ToString(), ct: ct);
    }
}
