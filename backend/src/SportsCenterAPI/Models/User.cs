using SportsCenterAPI.Constants;

namespace SportsCenterAPI.Models;

/// <summary>
/// Tài khoản đăng nhập. Map bảng dbo.[User].
/// </summary>
public class User
{
    public int Id { get; set; }

    public int RoleId { get; set; }
    public int CenterId { get; set; }

    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Hash mật khẩu (PBKDF2). Không trả field này ra API.
    /// </summary>
    public string PasswordHash { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;
    public string? Qualification { get; set; }

    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public DateOnly? DateOfBirth { get; set; }
    public string? Gender { get; set; }

    /// <summary>
    /// ACTIVE, INACTIVE hoặc SUSPENDED. Chỉ ACTIVE được đăng nhập.
    /// </summary>
    public string Status { get; set; } = UserStatuses.Active;

    public DateTime CreatedAt { get; set; }

    public Role Role { get; set; } = null!;
    public Center Center { get; set; } = null!;
}
