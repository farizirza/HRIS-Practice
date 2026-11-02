using HRIS.API.Common.Security;
using HRIS.API.Data;
using HRIS.API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HRIS.API.Extensions;

public static class DevelopmentSeeder
{
    /// <summary>
    /// Creates ONE bootstrap Admin account in Development so somebody can sign in and create the first HR user.
    /// The password comes from User Secrets (Seed:AdminPassword), never from source code.
    /// </summary>
    public static async Task SeedBootstrapAdminAsync(this WebApplication app)
    {
        var password = app.Configuration["Seed:AdminPassword"];
        if (string.IsNullOrWhiteSpace(password))
        {
            app.Logger.LogWarning("Seed:AdminPassword is not set (dotnet user-secrets set \"Seed:AdminPassword\" \"...\"): no Admin account was created");
            return;
        }

        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<HrisDbContext>();
        var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<User>>();

        const string email = "admin@hris.local";
        if (await db.Users.AnyAsync(u => u.Email == email))
            return;

        var admin = new User { Id = Guid.CreateVersion7(), Email = email, CreatedAt = DateTime.UtcNow };
        admin.PasswordHash = hasher.HashPassword(admin, password);
        admin.UserRoles.Add(new UserRole { UserId = admin.Id, RoleId = RoleIds.Admin });
        db.Users.Add(admin);
        await db.SaveChangesAsync();
    }
}