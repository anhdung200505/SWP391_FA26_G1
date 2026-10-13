namespace SportsCenterAPI.Models;

/// <summary>
/// Trung tâm thể thao. Map bảng dbo.Center.
/// </summary>
public class Center
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string? Hotline { get; set; }
    public string? Email { get; set; }

    // Danh sách user thuộc trung tâm này.
    public ICollection<User> Users { get; set; } = new List<User>();
}
