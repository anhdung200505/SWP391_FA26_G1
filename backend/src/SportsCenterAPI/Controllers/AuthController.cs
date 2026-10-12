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

    // =========================================================================
    // POST /api/auth/login
    // =========================================================================

    /// <summary>
    /// Authenticate a user and return a JWT access token.
    /// </summary>
    /// <response code="200">Login successful — returns access token and user info.</response>
    /// <response code="400">Validation error — missing or invalid input.</response>
    /// <response code="401">Invalid credentials or account not active.</response>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        // ModelState validation is handled automatically by [ApiController]
        // and returns 400 with ProblemDetails when annotations fail

        var result = await _authService.LoginAsync(request);

        if (result is null)
        {
            // Generic message regardless of whether username was wrong, password was wrong,
            // or account is inactive — prevents information leakage
            return Unauthorized(new { message = "Invalid username or password." });
        }

        return Ok(result);
    }

    // =========================================================================
    // POST /api/auth/logout
    // =========================================================================

    /// <summary>
    /// Logout the current user.
    /// Stateless logout: the server does not revoke the JWT.
    /// The issued access token remains valid until its "exp" claim expires.
    /// Client is responsible for discarding the token.
    /// </summary>
    /// <response code="200">Logout acknowledged. Client must discard the token.</response>
    /// <response code="401">No valid authentication token provided.</response>
    [HttpPost("logout")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult Logout()
    {
        // Stateless logout — no server-side token revocation.
        // The access JWT will remain valid until its "exp" claim elapses.
        // Client must discard the token on their end.

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        _logger.LogInformation("Logout acknowledged for UserId={UserId}", userId);

        return Ok(new
        {
            message = "Logged out successfully. Please discard your access token."
        });
    }

    // =========================================================================
    // GET /api/auth/me
    // =========================================================================

    /// <summary>
    /// Get the currently authenticated user's profile.
    /// Identity is determined from the JWT claims — client cannot specify a different user ID.
    /// </summary>
    /// <response code="200">Returns current user info.</response>
    /// <response code="401">Not authenticated or token invalid/expired.</response>
    /// <response code="404">User account no longer exists.</response>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(UserInfoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Me()
    {
        // User ID comes from JWT claims — NOT from query string or request body
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(userIdClaim, out var userId))
        {
            // This should not happen if JWT is properly configured and validated
            _logger.LogError("Invalid or missing NameIdentifier claim in validated JWT.");
            return Unauthorized(new { message = "Token does not contain a valid user identity." });
        }

        var userInfo = await _authService.GetCurrentUserAsync(userId);

        if (userInfo is null)
        {
            return NotFound(new { message = "User account not found." });
        }

        // Design note: If user was deactivated after token was issued,
        // we still return 200 with current status in the response body.
        // The caller can inspect userInfo.Status.
        // A stricter design would return 401/403 if Status != "ACTIVE".
        // This behaviour can be changed when the team agrees on a policy.

        return Ok(userInfo);
    }
}
