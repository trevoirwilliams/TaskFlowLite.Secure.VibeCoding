namespace TaskFlowLite.Application.Models.Auth;

public record AuthResponse(
    string AccessToken,
    string TokenType,
    DateTime ExpiresAtUtc,
    int UserId,
    string Email,
    string DisplayName);
