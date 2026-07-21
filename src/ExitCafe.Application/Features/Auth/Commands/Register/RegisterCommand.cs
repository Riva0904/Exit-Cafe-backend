using ExitCafe.Application.Features.Auth.Dtos;
using MediatR;

namespace ExitCafe.Application.Features.Auth.Commands.Register;

public record RegisterCommand(string Email, string Password, string FirstName, string LastName, string? PhoneNumber) : IRequest<AuthResponse>;
