namespace SportsCenterAPI.DTOs.Auth;

/// <summary>
/// Represents the authenticated user's public information.
/// Never includes password_hash or any sensitive internal data.
/// </summary>
public class UserInfoDto
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Gender { get; set; }
    public DateOnly? DateOfBirth { get; set; }
    public string Role { get; set; } = string.Empty;
    public int CenterId { get; set; }
    public string CenterName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}
