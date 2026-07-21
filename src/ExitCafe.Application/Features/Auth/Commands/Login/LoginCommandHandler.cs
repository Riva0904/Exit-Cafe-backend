using ExitCafe.Application.Common.Interfaces;
using ExitCafe.Application.Features.Auth.Dtos;
using ExitCafe.Domain.Entities;
using ExitCafe.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExitCafe.Application.Features.Auth.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponse>
{
    private readonly IUnitOfWork _uow;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IAuditLogService _auditLog;
    private readonly TokenIssuer _tokenIssuer;

    public LoginCommandHandler(IUnitOfWork uow, IPasswordHasher passwordHasher, IAuditLogService auditLog, TokenIssuer tokenIssuer)
    {
        _uow = uow;
        _passwordHasher = passwordHasher;
        _auditLog = auditLog;
        _tokenIssuer = tokenIssuer;
    }

    public async Task<AuthResponse> Handle(LoginCommand request, CancellationToken ct)
    {
        var user = await _uow.Users.Query().Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Email == request.Email, ct);

        if (user is null || !_passwordHasher.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedException("Invalid email or password.");

        if (!user.IsActive)
            throw new ForbiddenException("This account has been deactivated.");

        user.LastLoginAt = DateTime.UtcNow;
        _uow.Users.Update(user);
        await _uow.SaveChangesAsync(ct);
        await _auditLog.LogAsync("UserLoggedIn", nameof(User), user.Id.ToString(), ct: ct);

        return await _tokenIssuer.IssueAsync(user, user.Role.Name, request.IpAddress, ct);
    }
}
