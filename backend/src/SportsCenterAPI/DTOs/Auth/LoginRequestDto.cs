using System.ComponentModel.DataAnnotations;

namespace SportsCenterAPI.DTOs.Auth;

/// <summary>
/// Request body for POST /api/auth/login.
/// </summary>
public class LoginRequestDto
{
    [Required(ErrorMessage = "Username is required.")]
    [MaxLength(50, ErrorMessage = "Username must not exceed 50 characters.")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required.")]
    [MaxLength(128, ErrorMessage = "Password must not exceed 128 characters.")]
    public string Password { get; set; } = string.Empty;
}
