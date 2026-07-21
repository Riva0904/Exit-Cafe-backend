using ExitCafe.Application.Common.Interfaces;
using ExitCafe.Application.Features.Auth.Dtos;
using ExitCafe.Domain.Entities;
using ExitCafe.Domain.Exceptions;
using MediatR;

namespace ExitCafe.Application.Features.Auth.Commands.Register;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthResponse>
{
    private const string DefaultRoleName = "Customer";

    private readonly IUnitOfWork _uow;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IAuditLogService _auditLog;
    private readonly TokenIssuer _tokenIssuer;

    public RegisterCommandHandler(IUnitOfWork uow, IPasswordHasher passwordHasher, IAuditLogService auditLog, TokenIssuer tokenIssuer)
    {
        _uow = uow;
        _passwordHasher = passwordHasher;
        _auditLog = auditLog;
        _tokenIssuer = tokenIssuer;
    }

    public async Task<AuthResponse> Handle(RegisterCommand request, CancellationToken ct)
    {
        if (await _uow.Users.AnyAsync(u => u.Email == request.Email, ct))
            throw new ConflictException("A user with this email already exists.");

        var role = await _uow.Roles.FirstOrDefaultAsync(r => r.Name == DefaultRoleName, ct)
            ?? throw new BadRequestException("Default role not configured. Contact administrator.");

        var user = new User
        {
            Email = request.Email,
            PasswordHash = _passwordHasher.Hash(request.Password),
            FirstName = request.FirstName,
            LastName = request.LastName,
            PhoneNumber = request.PhoneNumber,
            RoleId = role.Id,
            IsActive = true
        };

        await _uow.Users.AddAsync(user, ct);

        var customer = new Customer
        {
            UserId = user.Id,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Phone = request.PhoneNumber,
            IsGuest = false
        };
        await _uow.Customers.AddAsync(customer, ct);

        await _uow.SaveChangesAsync(ct);
        await _auditLog.LogAsync("UserRegistered", nameof(User), user.Id.ToString(), ct: ct);

        return await _tokenIssuer.IssueAsync(user, role.Name, null, ct);
    }
}
