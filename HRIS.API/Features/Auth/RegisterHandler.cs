using FluentValidation;
using HRIS.API.Common.Exceptions;
using HRIS.API.Common.Security;
using HRIS.API.Data;
using HRIS.API.Entities;
using HRIS.API.Extensions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HRIS.API.Features.Auth;

/// <summary>Activate a login account for an existing employee.</summary>
public record RegisterCommand(string EmployeeNumber, string Email, string Password) : IRequest<RegisterResponse>;

public record RegisterResponse(Guid UserId, Guid EmployeeId, string Email, IReadOnlyCollection<string> Roles);

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.EmployeeNumber).NotEmpty();
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(256);
        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(8)
            .Matches("[A-Z]").WithMessage("Password must contain an uppercase letter.")
            .Matches("[a-z]").WithMessage("Password must contain a lowercase letter.")
            .Matches("[0-9]").WithMessage("Password must contain a digit.");
    }
}

public class RegisterHandler(HrisDbContext db, IPasswordHasher<User> passwordHasher, TimeProvider timeProvider, ILogger<RegisterHandler> logger)
    : IRequestHandler<RegisterCommand, RegisterResponse>
{
    public async Task<RegisterResponse> Handle(RegisterCommand command, CancellationToken ct)
    {
        var email = command.Email.NormalizeEmail();   // the helper from Module 7: trim + lower case
        var employee = await db.Employees.FirstOrDefaultAsync(
            e => e.EmployeeNumber == command.EmployeeNumber && e.Email == email && e.Status == EmployeeStatus.Active, ct);

        // Same message whether the number or the email is wrong: the caller cannot tell which one failed.
        if (employee is null)
            throw new BusinessRuleException("No active employee matches this employee number and email.");
        if (employee.UserId is not null || await db.Users.AnyAsync(u => u.Email == email, ct))
            throw new ConflictException("An account has already been activated for this employee.");

        var user = new User { Id = Guid.CreateVersion7(), Email = email, CreatedAt = timeProvider.GetUtcNow().UtcDateTime };
        user.PasswordHash = passwordHasher.HashPassword(user, command.Password);
        user.UserRoles.Add(new UserRole { UserId = user.Id, RoleId = RoleIds.Employee });   // the lowest role; an Admin grants more later

        db.Users.Add(user);
        employee.UserId = user.Id;
        try
        {
            await db.SaveChangesAsync(ct);   // User insert + Employee update in one transaction
        }
        catch (DbUpdateException ex)
        {
            // Two requests can pass the checks above at the same time; the unique index then rejects the second one.
            if (!ex.IsUniqueConstraintViolation())
                throw;   // not ours to translate: rethrow it unchanged

            throw new ConflictException("An account has already been activated for this employee.", ex);
        }

        logger.LogInformation("Account {UserId} activated for employee {EmployeeId}", user.Id, employee.Id);   // never log the password
        return new RegisterResponse(user.Id, employee.Id, user.Email, [Roles.Employee]);
    }
}