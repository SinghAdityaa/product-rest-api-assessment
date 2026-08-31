using Application.DTOs;

namespace Application.Interfaces;

public interface IAuthService
{
    Task<AuthResponse?> LoginAsync(LoginRequest request, CancellationToken ct);
    Task<AuthResponse?> RefreshAsync(RefreshRequest request, CancellationToken ct);
}
