using AutoMapper;
using ExitCafe.Application.Common.Interfaces;
using ExitCafe.Application.Features.Contact.Dtos;
using ExitCafe.Domain.Entities;
using ExitCafe.Domain.Exceptions;
using MediatR;

namespace ExitCafe.Application.Features.Contact.Commands.MarkContactMessageAsRead;

public class MarkContactMessageAsReadCommandHandler : IRequestHandler<MarkContactMessageAsReadCommand, ContactMessageDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public MarkContactMessageAsReadCommandHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<ContactMessageDto> Handle(MarkContactMessageAsReadCommand request, CancellationToken ct)
    {
        var message = await _uow.ContactMessages.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException(nameof(ContactMessage), request.Id);

        message.IsRead = true;
        _uow.ContactMessages.Update(message);
        await _uow.SaveChangesAsync(ct);
        return _mapper.Map<ContactMessageDto>(message);
    }
}
