namespace SportsCenterAPI.DTOs.Auth;

/// <summary>
/// Kết quả đăng nhập: token JWT và thông tin user.
/// </summary>
public class LoginResponseDto
{
    public string AccessToken { get; set; } = string.Empty;
    public string TokenType { get; set; } = "Bearer";

    /// <summary>
    /// Thời gian sống của token, tính bằng giây.
    /// </summary>
    public int ExpiresIn { get; set; }

    public UserInfoDto User { get; set; } = null!;
}
