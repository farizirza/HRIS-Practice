using System.Text;
using HRIS.API.Common.Security;
using HRIS.API.Entities;
using HRIS.API.Features.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace HRIS.API.Extensions;

public static class SecurityExtensions
{
    /// <summary>Module 10: password hashing, Basic authentication, roles, policies, fallback policy, current user.</summary>
    public static IServiceCollection AddHrisSecurity(this IServiceCollection services,
        IConfiguration configuration)
    {
        // Password hashing (PBKDF2 + random salt) and the reusable credential check
        services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
        services.AddScoped<IUserCredentialService, UserCredentialService>();

        // Authentication: who is calling? (Basic until Module 11 replaces it with JWT)
        services.AddAuthentication(BasicAuthenticationHandler.SchemeName)
            .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>(BasicAuthenticationHandler.SchemeName, options => { });

        // Authorization: what may the caller do?
        services.AddAuthorizationBuilder()
            .AddPolicy(Policies.HrOnly, policy => policy.RequireRole(Roles.Admin, Roles.Hr))
            .AddPolicy(Policies.HasEmployeeProfile, policy => policy.RequireClaim(HrisClaimTypes.EmployeeId))
            // Secure by default: an endpoint without [Authorize] still requires a signed-in user. Public endpoints need [AllowAnonymous].
            .SetFallbackPolicy(new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build());

        // The current user, available to handlers without touching HttpContext
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUser, CurrentUser>();
        
        // JWT options: bound from the "Jwt" section and checked once at startup
        services.AddOptions<JwtOptions>()
            .Bind(configuration.GetSection(JwtOptions.SectionName))
            .Validate(o => Encoding.UTF8.GetByteCount(o.Key) >= 32,
                "Jwt:Key must be at least 32 bytes (HS256). Set it with User Secrets.")
            .ValidateOnStart();
        services.AddSingleton<ITokenService, JwtTokenService>();
        return services;
    }
}