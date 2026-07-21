namespace ExitCafe.Application.Features.Auth.Dtos;

public record AuthResponse(Guid UserId, string Email, string FirstName, string LastName, string Role, string AccessToken, string RefreshToken, DateTime AccessTokenExpiresAt);
