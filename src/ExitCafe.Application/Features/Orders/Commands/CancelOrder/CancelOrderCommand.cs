using MediatR;

namespace ExitCafe.Application.Features.Orders.Commands.CancelOrder;

public record CancelOrderCommand(Guid Id) : IRequest;
