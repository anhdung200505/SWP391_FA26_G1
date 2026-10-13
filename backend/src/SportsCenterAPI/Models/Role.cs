namespace SportsCenterAPI.Models;

/// <summary>
/// Vai trò đăng nhập. Map bảng dbo.Role.
/// </summary>
public class Role
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    /// <summary>
    /// Map cột "create_at" (tên cột trong DB bị thiếu chữ d, không đổi).
    /// </summary>
    public DateTime CreateAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    // User đang giữ vai trò này.
    public ICollection<User> Users { get; set; } = new List<User>();
}
