using ExitCafe.Application.Common.Interfaces;
using ExitCafe.Application.DTOs.Auth;
using ExitCafe.Application.Services.Interfaces;
using ExitCafe.Domain.Entities;
using ExitCafe.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace ExitCafe.Application.Services.Implementations;

public class AuthService : IAuthService
{
    private const string DefaultRoleName = "Customer";
    private readonly IUnitOfWork _uow;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IAuditLogService _auditLog;
    private readonly IConfiguration _configuration;

    public AuthService(IUnitOfWork uow, IPasswordHasher passwordHasher, IJwtTokenService jwtTokenService,
        IAuditLogService auditLog, IConfiguration configuration)
    {
        _uow = uow;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
        _auditLog = auditLog;
        _configuration = configuration;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken ct = default)
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

        return await IssueTokensAsync(user, role.Name, null, ct);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request, string? ipAddress, CancellationToken ct = default)
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

        return await IssueTokensAsync(user, user.Role.Name, ipAddress, ct);
    }

    public async Task<AuthResponse> RefreshTokenAsync(string refreshToken, string? ipAddress, CancellationToken ct = default)
    {
        var existingToken = await _uow.RefreshTokens.Query().Include(t => t.User).ThenInclude(u => u.Role)
            .FirstOrDefaultAsync(t => t.Token == refreshToken, ct)
            ?? throw new UnauthorizedException("Invalid refresh token.");

        if (!existingToken.IsActive)
            throw new UnauthorizedException("Refresh token is expired or has been revoked.");

        existingToken.RevokedAt = DateTime.UtcNow;
        existingToken.RevokedByIp = ipAddress;

        var response = await IssueTokensAsync(existingToken.User, existingToken.User.Role.Name, ipAddress, ct, replaces: existingToken);
        return response;
    }

    public async Task RevokeTokenAsync(string refreshToken, string? ipAddress, CancellationToken ct = default)
    {
        var existingToken = await _uow.RefreshTokens.FirstOrDefaultAsync(t => t.Token == refreshToken, ct)
            ?? throw new NotFoundException("Refresh token not found.");

        existingToken.RevokedAt = DateTime.UtcNow;
        existingToken.RevokedByIp = ipAddress;
        _uow.RefreshTokens.Update(existingToken);
        await _uow.SaveChangesAsync(ct);
    }

    private async Task<AuthResponse> IssueTokensAsync(User user, string roleName, string? ipAddress, CancellationToken ct, RefreshToken? replaces = null)
    {
        var accessTokenMinutes = int.Parse(_configuration["Jwt:AccessTokenExpiryMinutes"] ?? "60");
        var refreshTokenDays = int.Parse(_configuration["Jwt:RefreshTokenExpiryDays"] ?? "7");

        var accessToken = _jwtTokenService.GenerateAccessToken(user.Id, user.Email, roleName);
        var refreshTokenValue = _jwtTokenService.GenerateRefreshToken();

        var refreshToken = new RefreshToken
        {
            UserId = user.Id,
            Token = refreshTokenValue,
            ExpiresAt = DateTime.UtcNow.AddDays(refreshTokenDays),
            CreatedByIp = ipAddress
        };

        await _uow.RefreshTokens.AddAsync(refreshToken, ct);

        if (replaces != null)
        {
            replaces.ReplacedByToken = refreshTokenValue;
            _uow.RefreshTokens.Update(replaces);
        }

        await _uow.SaveChangesAsync(ct);

        return new AuthResponse(user.Id, user.Email, user.FirstName, user.LastName, roleName,
            accessToken, refreshTokenValue, DateTime.UtcNow.AddMinutes(accessTokenMinutes));
    }
}
