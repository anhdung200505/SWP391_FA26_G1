using SportsCenterAPI.DTOs.Auth;

namespace SportsCenterAPI.Services;

/// <summary>
/// Contract for authentication business logic.
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Validates credentials and returns login response with access token.
    /// Returns null if credentials are invalid or account is not ACTIVE.
    /// </summary>
    Task<LoginResponseDto?> LoginAsync(LoginRequestDto request);

    /// <summary>
    /// Returns the currently authenticated user's public info by their ID (from JWT claims).
    /// Returns null if user is not found.
    /// </summary>
    Task<UserInfoDto?> GetCurrentUserAsync(int userId);
}
