using ExitCafe.Application.Features.Contact.Dtos;
using MediatR;

namespace ExitCafe.Application.Features.Contact.Commands.MarkContactMessageAsRead;

public record MarkContactMessageAsReadCommand(Guid Id) : IRequest<ContactMessageDto>;
