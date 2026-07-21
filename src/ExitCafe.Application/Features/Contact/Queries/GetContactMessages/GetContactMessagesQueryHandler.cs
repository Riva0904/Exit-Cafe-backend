using AutoMapper;
using ExitCafe.Application.Common.Interfaces;
using ExitCafe.Application.Common.Models;
using ExitCafe.Application.Features.Contact.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExitCafe.Application.Features.Contact.Queries.GetContactMessages;

public class GetContactMessagesQueryHandler : IRequestHandler<GetContactMessagesQuery, PagedResult<ContactMessageDto>>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public GetContactMessagesQueryHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<PagedResult<ContactMessageDto>> Handle(GetContactMessagesQuery request, CancellationToken ct)
    {
        var q = _uow.ContactMessages.Query();
        var totalCount = await q.CountAsync(ct);
        var items = await q.OrderByDescending(m => m.CreatedAt)
            .Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToListAsync(ct);

        return new PagedResult<ContactMessageDto>
        {
            Items = _mapper.Map<List<ContactMessageDto>>(items),
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }
}
