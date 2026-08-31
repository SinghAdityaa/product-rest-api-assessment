namespace Application.DTOs;

public sealed record LoginRequest(string Username, string Password);
public sealed record AuthResponse(string AccessToken, string RefreshToken, DateTime ExpiresAtUtc);
public sealed record RefreshRequest(string RefreshToken);
