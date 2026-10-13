namespace SportsCenterAPI.Configuration;

/// <summary>
/// Cấu hình JWT, đọc từ mục "JwtSettings".
/// Khóa ký lấy từ User Secrets hoặc biến môi trường, không ghi cứng trong file.
/// </summary>
public class JwtSettings
{
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;

    /// <summary>
    /// Số phút token còn hiệu lực. Mặc định 60.
    /// </summary>
    public int AccessTokenExpirationMinutes { get; set; } = 60;

    /// <summary>
    /// Khóa ký HMAC-SHA256, tối thiểu 32 ký tự.
    /// Biến môi trường: JwtSettings__SigningKey.
    /// </summary>
    public string SigningKey { get; set; } = string.Empty;
}
