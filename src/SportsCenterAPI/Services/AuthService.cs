using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SportsCenterAPI.Configuration;
using SportsCenterAPI.Data;
using SportsCenterAPI.DTOs.Auth;
using SportsCenterAPI.Models;

namespace SportsCenterAPI.Services;

public class AuthService : IAuthService
{
    private readonly SportsCenterDbContext _db;
    private readonly PasswordHasher<User> _passwordHasher;
    private readonly JwtSettings _jwtSettings;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        SportsCenterDbContext db,
        IOptions<JwtSettings> jwtOptions,
        ILogger<AuthService> logger)
    {
        _db = db;
        _passwordHasher = new PasswordHasher<User>();
        _jwtSettings = jwtOptions.Value;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto request)
    {
        // 1. Find user by username — include Role and Center for response data
        var user = await _db.Users
            .Include(u => u.Role)
            .Include(u => u.Center)
            .Where(u => u.Username == request.Username)
            .FirstOrDefaultAsync();

        // 2. User not found — return null with NO indication of why
        //    (prevents username enumeration)
        if (user is null)
        {
            _logger.LogWarning("Login attempt for unknown username. [Redacted for security]");
            return null;
        }

        // 3. Account must be ACTIVE. INACTIVE / SUSPENDED → same null return (no info leakage)
        if (user.Status != "ACTIVE")
        {
            _logger.LogWarning("Login attempt for non-ACTIVE account. UserId={UserId}", user.Id);
            return null;
        }

        // 4. Verify password using ASP.NET Core Identity PasswordHasher
        //    Compatible with V3 PBKDF2-SHA512 hashes stored in the database
        var verifyResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
        if (verifyResult == PasswordVerificationResult.Failed)
        {
            _logger.LogWarning("Failed password verification for UserId={UserId}", user.Id);
            return null;
        }

        // 5. Build JWT access token
        var accessToken = GenerateAccessToken(user);
        var expiresInSeconds = _jwtSettings.AccessTokenExpirationMinutes * 60;

        // 6. Build user info DTO — never include PasswordHash
        var userInfo = MapToUserInfoDto(user);

        _logger.LogInformation("Successful login for UserId={UserId}, Role={Role}", user.Id, user.Role.Name);

        return new LoginResponseDto
        {
            AccessToken = accessToken,
            TokenType = "Bearer",
            ExpiresIn = expiresInSeconds,
            User = userInfo
        };
    }

    /// <inheritdoc/>
    public async Task<UserInfoDto?> GetCurrentUserAsync(int userId)
    {
        var user = await _db.Users
            .Include(u => u.Role)
            .Include(u => u.Center)
            .Where(u => u.Id == userId)
            .FirstOrDefaultAsync();

        if (user is null)
            return null;

        // Re-check status: user could have been deactivated after token was issued.
        // We return the current status so the caller (controller) can decide to reject.
        return MapToUserInfoDto(user);
    }

    // =========================================================================
    // Private helpers
    // =========================================================================

    private string GenerateAccessToken(User user)
    {
        var keyBytes = Encoding.UTF8.GetBytes(_jwtSettings.SigningKey);
        var signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(keyBytes),
            SecurityAlgorithms.HmacSha256);

        // Claims embedded in the JWT — these drive role-based authorization
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            // ClaimTypes.Role is recognized by ASP.NET Core [Authorize(Roles = "...")]
            new Claim(ClaimTypes.Role, user.Role.Name),
            new Claim("center_id", user.CenterId.ToString()),
        };

        var now = DateTime.UtcNow;
        var expiry = now.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            notBefore: now,
            expires: expiry,
            signingCredentials: signingCredentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static UserInfoDto MapToUserInfoDto(User user) => new()
    {
        Id = user.Id,
        Username = user.Username,
        FullName = user.FullName,
        Email = user.Email,
        Phone = user.Phone,
        Gender = user.Gender,
        DateOfBirth = user.DateOfBirth,
        Role = user.Role.Name,
        CenterId = user.CenterId,
        CenterName = user.Center.Name,
        Status = user.Status
        // PasswordHash is intentionally NOT mapped
    };
}
