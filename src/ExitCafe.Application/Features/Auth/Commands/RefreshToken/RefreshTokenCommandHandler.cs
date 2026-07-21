using ExitCafe.Application.Common.Interfaces;
using ExitCafe.Application.Features.Auth.Dtos;
using ExitCafe.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExitCafe.Application.Features.Auth.Commands.RefreshToken;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, AuthResponse>
{
    private readonly IUnitOfWork _uow;
    private readonly TokenIssuer _tokenIssuer;

    public RefreshTokenCommandHandler(IUnitOfWork uow, TokenIssuer tokenIssuer)
    {
        _uow = uow;
        _tokenIssuer = tokenIssuer;
    }

    public async Task<AuthResponse> Handle(RefreshTokenCommand request, CancellationToken ct)
    {
        var existingToken = await _uow.RefreshTokens.Query().Include(t => t.User).ThenInclude(u => u.Role)
            .FirstOrDefaultAsync(t => t.Token == request.RefreshToken, ct)
            ?? throw new UnauthorizedException("Invalid refresh token.");

        if (!existingToken.IsActive)
            throw new UnauthorizedException("Refresh token is expired or has been revoked.");

        existingToken.RevokedAt = DateTime.UtcNow;
        existingToken.RevokedByIp = request.IpAddress;

        return await _tokenIssuer.IssueAsync(existingToken.User, existingToken.User.Role.Name, request.IpAddress, ct, replaces: existingToken);
    }
}
