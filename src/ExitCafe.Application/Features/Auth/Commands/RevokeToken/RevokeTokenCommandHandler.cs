using ExitCafe.Application.Common.Interfaces;
using ExitCafe.Domain.Exceptions;
using MediatR;

namespace ExitCafe.Application.Features.Auth.Commands.RevokeToken;

public class RevokeTokenCommandHandler : IRequestHandler<RevokeTokenCommand>
{
    private readonly IUnitOfWork _uow;

    public RevokeTokenCommandHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task Handle(RevokeTokenCommand request, CancellationToken ct)
    {
        var existingToken = await _uow.RefreshTokens.FirstOrDefaultAsync(t => t.Token == request.RefreshToken, ct)
            ?? throw new NotFoundException("Refresh token not found.");

        existingToken.RevokedAt = DateTime.UtcNow;
        existingToken.RevokedByIp = request.IpAddress;
        _uow.RefreshTokens.Update(existingToken);
        await _uow.SaveChangesAsync(ct);
    }
}
