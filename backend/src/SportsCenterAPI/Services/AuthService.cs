using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SportsCenterAPI.Configuration;
using SportsCenterAPI.Constants;
using SportsCenterAPI.Data;
using SportsCenterAPI.DTOs.Auth;
using SportsCenterAPI.Models;

namespace SportsCenterAPI.Services;

public class AuthService : IAuthService
{
    private readonly SportsCenterDbContext _db;
    private readonly PasswordHasher<User> _passwordHasher = new();
    private readonly JwtSettings _jwtSettings;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        SportsCenterDbContext db,
        IOptions<JwtSettings> jwtOptions,
        ILogger<AuthService> logger)
    {
        _db = db;
        _jwtSettings = jwtOptions.Value;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto request)
    {
        // Kèm Role và Center vì response cần tên vai trò và tên trung tâm.
        var user = await _db.Users
            .Include(u => u.Role)
            .Include(u => u.Center)
            .FirstOrDefaultAsync(u => u.Username == request.Username);

        // Cùng trả null cho mọi lý do thất bại để không lộ username có tồn tại hay không.
        if (user is null)
        {
            _logger.LogWarning("Login failed: unknown username.");
            return null;
        }

        if (user.Status != UserStatuses.Active)
        {
            _logger.LogWarning("Login failed: account is not active. UserId={UserId}", user.Id);
            return null;
        }

        // So khớp mật khẩu với hash PBKDF2 đã lưu trong database.
        var verifyResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
        if (verifyResult == PasswordVerificationResult.Failed)
        {
            _logger.LogWarning("Login failed: wrong password. UserId={UserId}", user.Id);
            return null;
        }

        _logger.LogInformation("Login succeeded. UserId={UserId}, Role={Role}", user.Id, user.Role.Name);

        return new LoginResponseDto
        {
            AccessToken = CreateAccessToken(user),
            TokenType = "Bearer",
            ExpiresIn = _jwtSettings.AccessTokenExpirationMinutes * 60,
            User = ToUserInfo(user)
        };
    }

    /// <inheritdoc/>
    public async Task<UserInfoDto?> GetCurrentUserAsync(int userId)
    {
        var user = await _db.Users
            .Include(u => u.Role)
            .Include(u => u.Center)
            .FirstOrDefaultAsync(u => u.Id == userId);

        // Vẫn trả hồ sơ khi tài khoản đã bị khóa. Controller để client tự xem Status.
        return user is null ? null : ToUserInfo(user);
    }

    // Tạo JWT. Claim role và center_id dùng cho phân quyền ở các API sau.
    private string CreateAccessToken(User user)
    {
        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SigningKey));
        var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            // NameIdentifier là claim ASP.NET đọc khi lấy id user hiện tại.
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role.Name),
            new Claim("center_id", user.CenterId.ToString()),
        };

        var now = DateTime.UtcNow;
        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            notBefore: now,
            expires: now.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    // Chỉ copy field public. PasswordHash cố ý không có ở đây.
    private static UserInfoDto ToUserInfo(User user) => new()
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
    };
}
