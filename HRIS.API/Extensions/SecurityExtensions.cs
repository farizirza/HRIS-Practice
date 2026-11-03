using System.Text;
using HRIS.API.Common.Security;
using HRIS.API.Entities;
using HRIS.API.Features.Auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace HRIS.API.Extensions;

public static class SecurityExtensions
{
    /// <summary>Modules 10-11: password hashing, JWT Bearer authentication, roles, policies, fallback policy, current user.</summary>
    public static IServiceCollection AddHrisSecurity(this IServiceCollection services, IConfiguration configuration)
    {
        // Password hashing (PBKDF2 + random salt) and the reusable credential check
        services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
        services.AddScoped<IUserCredentialService, UserCredentialService>();

        // JWT options: bound from the "Jwt" section and checked once at startup (fail fast if the key is missing or too short)
        services.AddOptions<JwtOptions>()
            .Bind(configuration.GetSection(JwtOptions.SectionName))
            .Validate(o => Encoding.UTF8.GetByteCount(o.Key) >= 32,
                "Jwt:Key must be at least 32 bytes (HS256). Set it with User Secrets.")
            .ValidateOnStart();
        services.AddSingleton<ITokenService, JwtTokenService>();

        // Authentication: who is calling? Every request must carry "Authorization: Bearer <token>".
        // The same JwtOptions that signed the token now validate it. They are read LAZILY (Configure<IOptions<JwtOptions>>),
        // not with configuration.Get<JwtOptions>() here: with an empty Jwt:Key the eager version would crash with
        // "IDX10703: key length is zero" before ValidateOnStart can print the clear message above.
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();
        services.AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
            .Configure<IOptions<JwtOptions>>((bearer, jwtOptions) =>
            {
                var jwt = jwtOptions.Value;
                bearer.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwt.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwt.Audience,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key)),
                    ClockSkew = TimeSpan.FromMinutes(1)   // default is 5 minutes of tolerance after exp
                };
            });

        // Authorization: what may the caller do?
        services.AddAuthorizationBuilder()
            .AddPolicy(Policies.HrOnly, policy => policy.RequireRole(Roles.Admin, Roles.Hr))
            .AddPolicy(Policies.HasEmployeeProfile, policy => policy.RequireClaim(HrisClaimTypes.EmployeeId))
            // Secure by default: an endpoint without [Authorize] still requires a signed-in user. Public endpoints need [AllowAnonymous].
            .SetFallbackPolicy(new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build());

        // The current user, available to handlers without touching HttpContext
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUser, CurrentUser>();
        return services;
    }
}