using HRIS.API.Data;
using HRIS.API.Entities;
using HRIS.API.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HRIS.API.Features.Auth;

public record AuthenticatedUser(Guid Id, string Email, string DisplayName, Guid? EmployeeId, IReadOnlyCollection<string> Roles);

public interface IUserCredentialService
{
    /// <summary>Returns the user when the email and password are correct and the account is active; otherwise null.</summary>
    Task<AuthenticatedUser?> ValidateAsync(string email, string password, CancellationToken ct = default);
}

public class UserCredentialService(HrisDbContext db, IPasswordHasher<User> passwordHasher, ILogger<UserCredentialService> logger)
    : IUserCredentialService
{
    public async Task<AuthenticatedUser?> ValidateAsync(string email, string password, CancellationToken ct = default)
    {
        var normalized = email.NormalizeEmail();
        var user = await db.Users
            .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
            .SingleOrDefaultAsync(u => u.Email == normalized, ct);

        // Failed sign-ins are Warnings so brute-force attempts are visible in the log.
        // Never log the password, and do not log the typed email (personal data; it may even be a password typed in the wrong box).
        if (user is null)
        {
            logger.LogWarning("Login rejected: no account for the given email");
            return null;
        }
        if (!user.IsActive)
        {
            logger.LogWarning("Login rejected for user {UserId}: account is deactivated", user.Id);
            return null;
        }

        var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
        if (result == PasswordVerificationResult.Failed)
        {
            logger.LogWarning("Login rejected for user {UserId}: wrong password", user.Id);
            return null;
        }
        if (result == PasswordVerificationResult.SuccessRehashNeeded)   // correct, but hashed with older settings: upgrade it now
        {
            user.PasswordHash = passwordHasher.HashPassword(user, password);
            await db.SaveChangesAsync(ct);
        }

        var employee = await db.Employees
            .Where(e => e.UserId == user.Id)
            .Select(e => new { e.Id, e.FullName })
            .FirstOrDefaultAsync(ct);

        return new AuthenticatedUser(
            user.Id, user.Email, employee?.FullName ?? user.Email, employee?.Id,
            user.UserRoles.Select(ur => ur.Role.Name).ToList());
    }
}