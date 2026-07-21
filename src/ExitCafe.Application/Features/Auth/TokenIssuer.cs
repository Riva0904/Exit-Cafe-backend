using ExitCafe.Application.Common.Interfaces;
using ExitCafe.Application.Features.Auth.Dtos;
using ExitCafe.Domain.Entities;
using Microsoft.Extensions.Configuration;

namespace ExitCafe.Application.Features.Auth;

public class TokenIssuer
{
    private readonly IUnitOfWork _uow;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IConfiguration _configuration;

    public TokenIssuer(IUnitOfWork uow, IJwtTokenService jwtTokenService, IConfiguration configuration)
    {
        _uow = uow;
        _jwtTokenService = jwtTokenService;
        _configuration = configuration;
    }

    public async Task<AuthResponse> IssueAsync(User user, string roleName, string? ipAddress, CancellationToken ct, RefreshToken? replaces = null)
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
