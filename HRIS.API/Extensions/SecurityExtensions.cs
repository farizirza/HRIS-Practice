using HRIS.API.Entities;
using HRIS.API.Features.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;

namespace HRIS.API.Extensions;

public static class SecurityExtensions
{
    /// <summary>Module 10: password hashing and Basic authentication. Lesson 6 adds policies and the current user.</summary>
    public static IServiceCollection AddHrisSecurity(this IServiceCollection services)
    {
        // Password hashing (PBKDF2 + random salt) and the reusable credential check
        services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
        services.AddScoped<IUserCredentialService, UserCredentialService>();   // Stage 1 replaces this line (see below)

        // Authentication: who is calling? (Basic until Module 11 replaces it with JWT)
        services.AddAuthentication(BasicAuthenticationHandler.SchemeName)
            .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>(BasicAuthenticationHandler.SchemeName, options => { });

        return services;
    }
}