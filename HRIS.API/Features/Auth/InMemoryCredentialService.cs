using HRIS.API.Common.Security;
using HRIS.API.Entities;
using HRIS.API.Extensions;
using Microsoft.AspNetCore.Identity;

namespace HRIS.API.Features.Auth;

/// <summary>Stage 1 of Lesson 5: three fixed users, no database. Replaced by UserCredentialService in Stage 2.</summary>
public class InMemoryCredentialService : IUserCredentialService
{
    private readonly List<(AuthenticatedUser User, string PasswordHash)> _users;

    public InMemoryCredentialService()
    {
        var hasher = new PasswordHasher<User>();
        _users =
        [
            (new AuthenticatedUser(Guid.Parse("11111111-0000-0000-0000-000000000001"), "admin@hris.local", "Admin", null, [Roles.Admin]), hasher.HashPassword(new User(), "Passw0rd!")),
            (new AuthenticatedUser(Guid.Parse("11111111-0000-0000-0000-000000000002"), "dewi@hris.local", "Dewi HR", null, [Roles.Hr]), hasher.HashPassword(new User(), "Passw0rd!")),
            (new AuthenticatedUser(Guid.Parse("11111111-0000-0000-0000-000000000003"), "siti@hris.local", "Siti Employee", null, [Roles.Employee]), hasher.HashPassword(new User(), "Passw0rd!"))
        ];
    }

    public Task<AuthenticatedUser?> ValidateAsync(string email, string password, CancellationToken ct = default)
    {
        var hasher = new PasswordHasher<User>();
        var match = _users.FirstOrDefault(u => u.User.Email == email.NormalizeEmail());
        if (match.User is null)
            return Task.FromResult<AuthenticatedUser?>(null);

        var ok = hasher.VerifyHashedPassword(new User(), match.PasswordHash, password) != PasswordVerificationResult.Failed;
        return Task.FromResult(ok ? match.User : null);
    }
}