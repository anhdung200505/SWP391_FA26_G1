namespace SportsCenterAPI.Models;

/// <summary>
/// Maps to dbo.Center in SportsCenterDB.
/// </summary>
public class Center
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string? Hotline { get; set; }
    public string? Email { get; set; }

    // Navigation
    public ICollection<User> Users { get; set; } = new List<User>();
}
