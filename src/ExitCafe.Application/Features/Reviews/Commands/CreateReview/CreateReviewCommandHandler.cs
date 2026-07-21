using AutoMapper;
using ExitCafe.Application.Common.Interfaces;
using ExitCafe.Application.Features.Reviews.Dtos;
using ExitCafe.Domain.Entities;
using ExitCafe.Domain.Enums;
using ExitCafe.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExitCafe.Application.Features.Reviews.Commands.CreateReview;

public class CreateReviewCommandHandler : IRequestHandler<CreateReviewCommand, ReviewDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUser;

    public CreateReviewCommandHandler(IUnitOfWork uow, IMapper mapper, ICurrentUserService currentUser)
    {
        _uow = uow;
        _mapper = mapper;
        _currentUser = currentUser;
    }

    public async Task<ReviewDto> Handle(CreateReviewCommand request, CancellationToken ct)
    {
        var userId = _currentUser.UserId ?? throw new UnauthorizedException();
        var customer = await _uow.Customers.FirstOrDefaultAsync(c => c.UserId == userId, ct)
            ?? throw new ForbiddenException("Your account has no customer profile, so it can't rate orders.");

        var order = await _uow.Orders.Query().Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.Id == request.OrderId, ct)
            ?? throw new NotFoundException(nameof(Order), request.OrderId);

        if (order.CustomerId != customer.Id)
            throw new ForbiddenException("This order does not belong to you.");

        if (order.Status != OrderStatus.Delivered)
            throw new BadRequestException("You can only rate items from delivered orders.");

        if (!order.OrderItems.Any(i => i.ProductId == request.ProductId))
            throw new BadRequestException("This product was not part of that order.");

        if (await _uow.Reviews.AnyAsync(r => r.CustomerId == customer.Id && r.ProductId == request.ProductId && r.OrderId == request.OrderId, ct))
            throw new ConflictException("You've already rated this item.");

        var review = new Review
        {
            ProductId = request.ProductId,
            CustomerId = customer.Id,
            OrderId = request.OrderId,
            Rating = request.Rating,
            Comment = request.Comment,
        };
        await _uow.Reviews.AddAsync(review, ct);
        await _uow.SaveChangesAsync(ct);

        var ratings = await _uow.Reviews.Query().Where(r => r.ProductId == request.ProductId).Select(r => r.Rating).ToListAsync(ct);
        var product = await _uow.Products.GetByIdAsync(request.ProductId, ct)
            ?? throw new NotFoundException(nameof(Product), request.ProductId);
        product.AverageRating = Math.Round((decimal)ratings.Average(), 1);
        product.ReviewCount = ratings.Count;
        // No explicit Update(): see UpdateProductCommandHandler — would wipe the product's Images.
        await _uow.SaveChangesAsync(ct);

        return _mapper.Map<ReviewDto>(review);
    }
}
