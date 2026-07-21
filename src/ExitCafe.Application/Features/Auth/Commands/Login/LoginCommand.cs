using ExitCafe.Application.Features.Auth.Dtos;
using MediatR;

namespace ExitCafe.Application.Features.Auth.Commands.Login;

public record LoginCommand(string Email, string Password) : IRequest<AuthResponse>
{
    public string? IpAddress { get; init; }
}
