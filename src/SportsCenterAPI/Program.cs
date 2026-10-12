using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SportsCenterAPI.Configuration;
using SportsCenterAPI.Data;
using SportsCenterAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// =========================================================================
// 1. Configuration — bind JwtSettings
//    Signing key is loaded from User Secrets (dev) or environment variable:
//    Environment variable name: JwtSettings__SigningKey
// =========================================================================
builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("JwtSettings"));

var jwtSettings = builder.Configuration
    .GetSection("JwtSettings")
    .Get<JwtSettings>()
    ?? throw new InvalidOperationException("JwtSettings section is missing from configuration.");

if (string.IsNullOrWhiteSpace(jwtSettings.SigningKey))
    throw new InvalidOperationException(
        "JwtSettings:SigningKey is not configured. " +
        "Set it via User Secrets: dotnet user-secrets set \"JwtSettings:SigningKey\" \"<your-key>\" " +
        "or via environment variable JwtSettings__SigningKey.");

// =========================================================================
// 2. Database — EF Core with SQL Server
//    Connection string from User Secrets or environment variable:
//    ConnectionStrings__SportsCenterDB
// =========================================================================
builder.Services.AddDbContext<SportsCenterDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("SportsCenterDB");
    if (string.IsNullOrWhiteSpace(connectionString))
        throw new InvalidOperationException(
            "ConnectionStrings:SportsCenterDB is not configured. " +
            "Set it via User Secrets or environment variable ConnectionStrings__SportsCenterDB.");

    options.UseSqlServer(connectionString);
});

// =========================================================================
// 3. Authentication — JWT Bearer
// =========================================================================
var keyBytes = Encoding.UTF8.GetBytes(jwtSettings.SigningKey);

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
        IssuerSigningKey = new SymmetricSecurityKey(keyBytes),

        ValidateIssuer = true,
        ValidIssuer = jwtSettings.Issuer,

        ValidateAudience = true,
        ValidAudience = jwtSettings.Audience,

        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero  // No tolerance — exact expiry
    };

    // Log authentication failures without revealing token content
    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            var logger = context.HttpContext.RequestServices
                .GetRequiredService<ILogger<Program>>();
            logger.LogWarning("JWT authentication failed: {ErrorType}",
                context.Exception.GetType().Name);
            return Task.CompletedTask;
        }
    };
});

// =========================================================================
// 4. Authorization — Role-Based
// =========================================================================
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("CenterManagerOnly",
        policy => policy.RequireRole("CENTER_MANAGER"));
    options.AddPolicy("CoachOnly",
        policy => policy.RequireRole("COACH"));
    options.AddPolicy("ReceptionistOnly",
        policy => policy.RequireRole("RECEPTIONIST"));
    options.AddPolicy("MemberOnly",
        policy => policy.RequireRole("MEMBER"));
    options.AddPolicy("StaffOnly",
        policy => policy.RequireRole("CENTER_MANAGER", "COACH", "RECEPTIONIST"));
});

// =========================================================================
// 5. Application Services
// =========================================================================
builder.Services.AddScoped<IAuthService, AuthService>();

// =========================================================================
// 6. Controllers
// =========================================================================
builder.Services.AddControllers();

// =========================================================================
// 7. Swagger / OpenAPI — with Bearer token support
// =========================================================================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Sports Center API",
        Version = "v1",
        Description = "Authentication API for Sports Center Management System — SWP391 Group 1"
    });

    // Add Bearer token authentication to Swagger UI
    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Enter JWT Bearer token: **Bearer {token}**",
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

// =========================================================================
// 8. CORS — configure for known frontend origins only
//    Update AllowedOrigins in appsettings when frontend URL is known
// =========================================================================
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", policy =>
    {
        var allowedOrigins = builder.Configuration
            .GetSection("AllowedOrigins")
            .Get<string[]>() ?? Array.Empty<string>();

        if (allowedOrigins.Length > 0)
            policy.WithOrigins(allowedOrigins)
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        else
            // Development fallback only — restrict in production
            policy.WithOrigins("http://localhost:3000", "http://localhost:5173")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
    });
});

// =========================================================================
// Build
// =========================================================================
var app = builder.Build();

// =========================================================================
// Middleware Pipeline
// =========================================================================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Sports Center API v1");
        c.RoutePrefix = "swagger"; // Access at /swagger
    });
}

app.UseHttpsRedirection();
app.UseCors("FrontendPolicy");

// Order matters: Authentication must come before Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
