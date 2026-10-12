namespace SportsCenterAPI.Models;

/// <summary>
/// Maps to dbo.[User] in SportsCenterDB.
/// "User" is a reserved keyword in SQL Server — handled via [Table] attribute in Fluent API.
/// </summary>
public class User
{
    public int Id { get; set; }

    public int RoleId { get; set; }
    public int CenterId { get; set; }

    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// ASP.NET Core Identity V3 PBKDF2-SHA512 hash.
    /// Never expose this field in any API response.
    /// </summary>
    public string PasswordHash { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;
    public string? Qualification { get; set; }

    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public DateOnly? DateOfBirth { get; set; }
    public string? Gender { get; set; }

    /// <summary>
    /// Allowed values: "ACTIVE", "INACTIVE", "SUSPENDED"
    /// Enforced by CHECK constraint in database.
    /// </summary>
    public string Status { get; set; } = "ACTIVE";

    public DateTime CreatedAt { get; set; }

    // Navigation properties
    public Role Role { get; set; } = null!;
    public Center Center { get; set; } = null!;
}
