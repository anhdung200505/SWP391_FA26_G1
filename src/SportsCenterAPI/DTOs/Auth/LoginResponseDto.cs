namespace SportsCenterAPI.DTOs.Auth;

/// <summary>
/// Response body for POST /api/auth/login.
/// Includes JWT access token and minimal user info.
/// </summary>
public class LoginResponseDto
{
    public string AccessToken { get; set; } = string.Empty;
    public string TokenType { get; set; } = "Bearer";

    /// <summary>
    /// Access token lifetime in seconds.
    /// </summary>
    public int ExpiresIn { get; set; }

    public UserInfoDto User { get; set; } = null!;
}
