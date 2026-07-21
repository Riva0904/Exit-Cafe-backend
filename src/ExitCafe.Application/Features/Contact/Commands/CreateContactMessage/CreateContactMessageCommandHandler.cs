using AutoMapper;
using ExitCafe.Application.Common.Interfaces;
using ExitCafe.Application.Features.Contact.Dtos;
using ExitCafe.Domain.Entities;
using MediatR;

namespace ExitCafe.Application.Features.Contact.Commands.CreateContactMessage;

public class CreateContactMessageCommandHandler : IRequestHandler<CreateContactMessageCommand, ContactMessageDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public CreateContactMessageCommandHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<ContactMessageDto> Handle(CreateContactMessageCommand request, CancellationToken ct)
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
}
