using ExitCafe.Application.Features.Contact.Dtos;
using MediatR;

namespace ExitCafe.Application.Features.Contact.Commands.CreateContactMessage;

public record CreateContactMessageCommand(string Name, string Email, string? Phone, string Subject, string Message) : IRequest<ContactMessageDto>;
