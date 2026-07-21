using MediatR;

namespace ExitCafe.Application.Features.Auth.Commands.RevokeToken;

public record RevokeTokenCommand(string RefreshToken) : IRequest
{
    public string? IpAddress { get; init; }
}
