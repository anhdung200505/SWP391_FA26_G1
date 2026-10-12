namespace SportsCenterAPI.Models;

/// <summary>
/// Maps to dbo.Role in SportsCenterDB.
/// NOTE: Column is named "create_at" (not "created_at") in the database — mapped via Fluent API.
/// </summary>
public class Role
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    /// <summary>
    /// Maps to the "create_at" column (typo in original schema — DO NOT rename the column).
    /// </summary>
    public DateTime CreateAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    // Navigation
    public ICollection<User> Users { get; set; } = new List<User>();
}
