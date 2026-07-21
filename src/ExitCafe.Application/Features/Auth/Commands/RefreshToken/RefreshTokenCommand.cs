using ExitCafe.Application.Features.Auth.Dtos;
using MediatR;

namespace ExitCafe.Application.Features.Auth.Commands.RefreshToken;

public record RefreshTokenCommand(string RefreshToken) : IRequest<AuthResponse>
{
    public string? IpAddress { get; init; }
}
