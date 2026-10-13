using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SportsCenterAPI.Configuration;
using SportsCenterAPI.Constants;
using SportsCenterAPI.Data;
using SportsCenterAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// 1. JWT: issuer/audience lấy từ appsettings, khóa ký lấy từ User Secrets hoặc biến môi trường.
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>()
    ?? throw new InvalidOperationException("Thiếu mục JwtSettings trong cấu hình.");

if (string.IsNullOrWhiteSpace(jwtSettings.SigningKey))
{
    throw new InvalidOperationException(
        "Chưa có JwtSettings:SigningKey. Đặt bằng User Secrets hoặc biến môi trường JwtSettings__SigningKey.");
}

if (jwtSettings.SigningKey.Length < 32)
{
    throw new InvalidOperationException(
        "JwtSettings:SigningKey phải dài ít nhất 32 ký tự để ký HMAC-SHA256.");
}

// 2. Database: chuỗi kết nối không để trong appsettings.
builder.Services.AddDbContext<SportsCenterDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("SportsCenterDB");
    if (string.IsNullOrWhiteSpace(connectionString))
    {
        throw new InvalidOperationException(
            "Chưa có ConnectionStrings:SportsCenterDB. Đặt bằng User Secrets hoặc biến môi trường ConnectionStrings__SportsCenterDB.");
    }

    options.UseSqlServer(connectionString);
});

// 3. Xác thực JWT. Token sai người ký, issuer, audience hoặc hết hạn thì từ chối.
var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SigningKey));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = signingKey,
        ValidateIssuer = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidateAudience = true,
        ValidAudience = jwtSettings.Audience,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };

    // Chỉ ghi loại lỗi, không ghi nội dung token.
    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
            logger.LogWarning("JWT authentication failed: {ErrorType}", context.Exception.GetType().Name);
            return Task.CompletedTask;
        }
    };
});

// 4. Policy theo vai trò, gắn lên controller khi có API tương ứng.
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("CenterManagerOnly", policy => policy.RequireRole(RoleNames.CenterManager));
    options.AddPolicy("CoachOnly", policy => policy.RequireRole(RoleNames.Coach));
    options.AddPolicy("ReceptionistOnly", policy => policy.RequireRole(RoleNames.Receptionist));
    options.AddPolicy("MemberOnly", policy => policy.RequireRole(RoleNames.Member));
    options.AddPolicy("StaffOnly", policy => policy.RequireRole(
        RoleNames.CenterManager, RoleNames.Coach, RoleNames.Receptionist));
});

// 5. Service nghiệp vụ, rồi bật controller.
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddControllers();

// 6. Swagger, kèm ô nhập token Bearer.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Sports Center API",
        Version = "v1",
        Description = "API xác thực cho hệ thống quản lý trung tâm thể thao — SWP391 Nhóm 1"
    });

    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Nhập token dạng: Bearer {token}",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        Reference = new OpenApiReference
        {
            Id = JwtBearerDefaults.AuthenticationScheme,
            Type = ReferenceType.SecurityScheme
        }
    };

    options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, securityScheme);
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { securityScheme, Array.Empty<string>() }
    });
});

// 7. Chỉ cho frontend đã khai báo. Chưa cấu hình thì dùng localhost lúc dev.
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", policy =>
    {
        var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>()
            ?? Array.Empty<string>();

        var origins = allowedOrigins.Length > 0
            ? allowedOrigins
            : new[] { "http://localhost:3000", "http://localhost:5173" };

        policy.WithOrigins(origins)
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Swagger chỉ mở khi chạy Development.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Sports Center API v1");
        c.RoutePrefix = "swagger";
    });
}

// Thứ tự bắt buộc: CORS, rồi xác thực, rồi phân quyền, rồi controller.
app.UseHttpsRedirection();
app.UseCors("FrontendPolicy");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
