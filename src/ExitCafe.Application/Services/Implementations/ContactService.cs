using AutoMapper;
using ExitCafe.Application.Common.Interfaces;
using ExitCafe.Application.Common.Models;
using ExitCafe.Application.DTOs.Contact;
using ExitCafe.Application.Services.Interfaces;
using ExitCafe.Domain.Entities;
using ExitCafe.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace ExitCafe.Application.Services.Implementations;

public class ContactService : IContactService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public ContactService(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<ContactMessageDto> CreateAsync(CreateContactMessageRequest request, CancellationToken ct = default)
    {
        var message = new ContactMessage
        {
            Name = request.Name,
            Email = request.Email,
            Phone = request.Phone,
            Subject = request.Subject,
            Message = request.Message,
            IsRead = false,
        };

        await _uow.ContactMessages.AddAsync(message, ct);
        await _uow.SaveChangesAsync(ct);
        return _mapper.Map<ContactMessageDto>(message);
    }

    public async Task<PagedResult<ContactMessageDto>> GetAllAsync(PaginationParams query, CancellationToken ct = default)
    {
        var q = _uow.ContactMessages.Query();
        var totalCount = await q.CountAsync(ct);
        var items = await q.OrderByDescending(m => m.CreatedAt)
            .Skip((query.PageNumber - 1) * query.PageSize).Take(query.PageSize).ToListAsync(ct);

        return new PagedResult<ContactMessageDto>
        {
            Items = _mapper.Map<List<ContactMessageDto>>(items),
            PageNumber = query.PageNumber,
            PageSize = query.PageSize,
            TotalCount = totalCount
        };
    }

    public async Task<ContactMessageDto> MarkAsReadAsync(Guid id, CancellationToken ct = default)
    {
        var message = await _uow.ContactMessages.GetByIdAsync(id, ct)
            ?? throw new NotFoundException(nameof(ContactMessage), id);

        message.IsRead = true;
        _uow.ContactMessages.Update(message);
        await _uow.SaveChangesAsync(ct);
        return _mapper.Map<ContactMessageDto>(message);
    }
}
