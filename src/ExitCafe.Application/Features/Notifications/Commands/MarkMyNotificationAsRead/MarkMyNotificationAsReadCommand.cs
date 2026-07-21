using MediatR;

namespace ExitCafe.Application.Features.Notifications.Commands.MarkMyNotificationAsRead;

public record MarkMyNotificationAsReadCommand(Guid Id) : IRequest;
