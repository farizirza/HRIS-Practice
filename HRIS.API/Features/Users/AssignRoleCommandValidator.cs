using FluentValidation;
using HRIS.API.Common.Exceptions;
using HRIS.API.Data;
using HRIS.API.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRIS.API.Features.Users;

/// <summary>Grant a role to a user.</summary>
public record AssignRoleCommand(Guid UserId, string Role) : IRequest;

public class AssignRoleCommandValidator : AbstractValidator<AssignRoleCommand>
{
    public AssignRoleCommandValidator(HrisDbContext db)
    {
        RuleFor(x => x.Role).NotEmpty()
            .MustAsync(async (name, ct) => await db.Roles.AnyAsync(r => r.Name == name, ct))
            .WithMessage("Unknown role.");
    }
}

public class AssignRoleHandler(HrisDbContext db, ILogger<AssignRoleHandler> logger) : IRequestHandler<AssignRoleCommand>
{
    public async Task Handle(AssignRoleCommand command, CancellationToken ct)
    {
        var user = await db.Users.Include(u => u.UserRoles).FirstOrDefaultAsync(u => u.Id == command.UserId, ct)
                   ?? throw new NotFoundException("User", command.UserId);
        var role = await db.Roles.SingleAsync(r => r.Name == command.Role, ct);

        if (user.UserRoles.Any(ur => ur.RoleId == role.Id))
            throw new ConflictException($"User already has the role '{role.Name}'.");

        db.UserRoles.Add(new UserRole { UserId = user.Id, RoleId = role.Id });
        await db.SaveChangesAsync(ct);
        logger.LogInformation("Role {Role} granted to user {UserId}", role.Name, user.Id);
    }
}