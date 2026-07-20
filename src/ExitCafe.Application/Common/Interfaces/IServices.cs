namespace ExitCafe.Application.Common.Interfaces;

public interface ICurrentUserService
{
    Guid? UserId { get; }
    string? Email { get; }
    string? Role { get; }
    string? IpAddress { get; }
    bool IsAuthenticated { get; }
}

public interface IPasswordHasher
{
    string Hash(string password);
    bool Verify(string password, string hash);
}

public interface IJwtTokenService
{
    string GenerateAccessToken(Guid userId, string email, string role);
    string GenerateRefreshToken();
    Guid? ValidateAccessTokenAndGetUserId(string token);
}

public interface IAuditLogService
{
    Task LogAsync(string action, string entityName, string? entityId, string? changes = null, CancellationToken ct = default);
}
