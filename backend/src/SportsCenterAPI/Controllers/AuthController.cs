using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportsCenterAPI.DTOs.Auth;
using SportsCenterAPI.Services;

namespace SportsCenterAPI.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    /// <summary>
    /// Đăng nhập, trả access token và thông tin user.
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        // [ApiController] tự trả 400 khi thiếu username hoặc password.
        var result = await _authService.LoginAsync(request);
        if (result is null)
            return Unauthorized(new { message = "Invalid username or password." });

        return Ok(result);
    }

    /// <summary>
    /// Đăng xuất. Server không thu hồi JWT; client phải xóa token.
    /// </summary>
    [HttpPost("logout")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult Logout()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        _logger.LogInformation("Logout acknowledged. UserId={UserId}", userId);

        return Ok(new
        {
            message = "Logged out successfully. Please discard your access token."
        });
    }

    /// <summary>
    /// Hồ sơ của user đang đăng nhập. Id lấy từ token, không nhận từ client.
    /// </summary>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(UserInfoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Me()
    {
        if (!TryGetUserId(out var userId))
        {
            _logger.LogError("JWT is missing a valid user id claim.");
            return Unauthorized(new { message = "Token does not contain a valid user identity." });
        }

        var userInfo = await _authService.GetCurrentUserAsync(userId);
        if (userInfo is null)
            return NotFound(new { message = "User account not found." });

        return Ok(userInfo);
    }

    // Đọc id từ claim của JWT đã được xác thực.
    private bool TryGetUserId(out int userId)
    {
        var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(claim, out userId);
    }
}
