using SportsCenterAPI.DTOs.Auth;

namespace SportsCenterAPI.Services;

/// <summary>
/// Nghiệp vụ đăng nhập và lấy hồ sơ user hiện tại.
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Kiểm tra tài khoản và trả token. Null nếu sai mật khẩu hoặc tài khoản không ACTIVE.
    /// </summary>
    Task<LoginResponseDto?> LoginAsync(LoginRequestDto request);

    /// <summary>
    /// Lấy hồ sơ public theo id trong JWT. Null nếu không còn user đó.
    /// </summary>
    Task<UserInfoDto?> GetCurrentUserAsync(int userId);
}
