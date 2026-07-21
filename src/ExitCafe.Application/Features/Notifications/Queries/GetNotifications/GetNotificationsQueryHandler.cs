using AutoMapper;
using ExitCafe.Application.Common.Interfaces;
using ExitCafe.Application.Common.Models;
using ExitCafe.Application.Features.Notifications.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExitCafe.Application.Features.Notifications.Queries.GetNotifications;

public class GetNotificationsQueryHandler : IRequestHandler<GetNotificationsQuery, PagedResult<NotificationDto>>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public GetNotificationsQueryHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<PagedResult<NotificationDto>> Handle(GetNotificationsQuery request, CancellationToken ct)
    {
        var q = _uow.Notifications.Query().Where(n => n.CustomerId == null);
        var totalCount = await q.CountAsync(ct);
        var items = await q.OrderByDescending(n => n.CreatedAt)
            .Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToListAsync(ct);

        return new PagedResult<NotificationDto>
        {
            Items = _mapper.Map<List<NotificationDto>>(items),
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }
}
