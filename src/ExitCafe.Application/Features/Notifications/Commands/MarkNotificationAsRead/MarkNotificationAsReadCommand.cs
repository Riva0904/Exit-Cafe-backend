using MediatR;

namespace ExitCafe.Application.Features.Notifications.Commands.MarkNotificationAsRead;

public record MarkNotificationAsReadCommand(Guid Id) : IRequest;
