using ExitCafe.Application.Common.Models;
using ExitCafe.Application.Features.Auth.Commands.Login;
using ExitCafe.Application.Features.Auth.Commands.RefreshToken;
using ExitCafe.Application.Features.Auth.Commands.Register;
using ExitCafe.Application.Features.Auth.Commands.RevokeToken;
using ExitCafe.Application.Features.Auth.Dtos;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ExitCafe.WebApi.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    private string? IpAddress => HttpContext.Connection.RemoteIpAddress?.ToString();

    [HttpPost("register")]
    public async Task<ActionResult<ApiResponse<AuthResponse>>> Register(RegisterCommand command, CancellationToken ct)
    {
        var result = await _mediator.Send(command, ct);
        return Ok(ApiResponse<AuthResponse>.Ok(result, "Registration successful."));
    }

    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse<AuthResponse>>> Login(LoginCommand command, CancellationToken ct)
    {
        var result = await _mediator.Send(command with { IpAddress = IpAddress }, ct);
        return Ok(ApiResponse<AuthResponse>.Ok(result, "Login successful."));
    }

    [HttpPost("refresh-token")]
    public async Task<ActionResult<ApiResponse<AuthResponse>>> RefreshToken(RefreshTokenCommand command, CancellationToken ct)
    {
        var result = await _mediator.Send(command with { IpAddress = IpAddress }, ct);
        return Ok(ApiResponse<AuthResponse>.Ok(result, "Token refreshed."));
    }

    [HttpPost("revoke-token")]
    public async Task<ActionResult<ApiResponse<object>>> RevokeToken(RevokeTokenCommand command, CancellationToken ct)
    {
        await _mediator.Send(command with { IpAddress = IpAddress }, ct);
        return Ok(ApiResponse<object>.Ok(new { }, "Token revoked."));
    }
}
