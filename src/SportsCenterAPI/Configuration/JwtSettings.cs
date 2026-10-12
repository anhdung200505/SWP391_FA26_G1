namespace SportsCenterAPI.Configuration;

/// <summary>
/// Bound from appsettings.json section "JwtSettings".
/// Signing key must be provided via User Secrets or environment variables — never hard-coded.
/// </summary>
public class JwtSettings
{
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;

    /// <summary>
    /// Access token lifetime in minutes. Default: 60.
    /// </summary>
    public int AccessTokenExpirationMinutes { get; set; } = 60;

    /// <summary>
    /// Loaded from User Secrets or environment variable "JwtSettings__SigningKey".
    /// Must be at least 32 characters (256 bits) for HMAC-SHA256.
    /// </summary>
    public string SigningKey { get; set; } = string.Empty;
}
