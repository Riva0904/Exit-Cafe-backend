using AutoMapper;
using ExitCafe.Application.Common.Interfaces;
using ExitCafe.Application.Features.Notifications.Dtos;
using ExitCafe.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExitCafe.Application.Features.Notifications.Queries.GetMyNotifications;

public class GetMyNotificationsQueryHandler : IRequestHandler<GetMyNotificationsQuery, List<NotificationDto>>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUser;

    public GetMyNotificationsQueryHandler(IUnitOfWork uow, IMapper mapper, ICurrentUserService currentUser)
    {
        _uow = uow;
        _mapper = mapper;
        _currentUser = currentUser;
    }

    public async Task<List<NotificationDto>> Handle(GetMyNotificationsQuery request, CancellationToken ct)
    {
        var userId = _currentUser.UserId ?? throw new UnauthorizedException();
        var customer = await _uow.Customers.FirstOrDefaultAsync(c => c.UserId == userId, ct);
        if (customer is null) return new List<NotificationDto>();

        var notifications = await _uow.Notifications.Query()
            .Where(n => n.CustomerId == customer.Id)
            .OrderByDescending(n => n.CreatedAt)
            .Take(20)
            .ToListAsync(ct);

        return _mapper.Map<List<NotificationDto>>(notifications);
    }
}
