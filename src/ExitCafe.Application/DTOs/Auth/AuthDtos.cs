namespace ExitCafe.Application.DTOs.Auth;

public record RegisterRequest(string Email, string Password, string FirstName, string LastName, string? PhoneNumber);

public record LoginRequest(string Email, string Password);

public record RefreshTokenRequest(string RefreshToken);

public record ForgotPasswordRequest(string Email);

public record ResetPasswordRequest(string Email, string Token, string NewPassword);

public record AuthResponse(Guid UserId, string Email, string FirstName, string LastName, string Role, string AccessToken, string RefreshToken, DateTime AccessTokenExpiresAt);
