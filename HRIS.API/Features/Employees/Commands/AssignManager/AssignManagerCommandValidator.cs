using FluentValidation;
using HRIS.API.Data;
using Microsoft.EntityFrameworkCore;

namespace HRIS.API.Features.Employees.Commands.AssignManager;

public class AssignManagerCommandValidator : AbstractValidator<AssignManagerCommand>
{
    public AssignManagerCommandValidator(HrisDbContext db)
    {
        // ManagerId = null means "no manager" and is allowed, so both rules run only when a manager is sent.
        RuleFor(x => x.ManagerId)
            .NotEqual(x => x.EmployeeId).WithMessage("An employee cannot be their own manager.")
            .When(x => x.ManagerId is not null);

        RuleFor(x => x.ManagerId)
            .MustAsync(async (id, ct) => await db.Employees.AnyAsync(e => e.Id == id, ct))
            .WithMessage("Manager does not exist.")
            .When(x => x.ManagerId is not null);
    }
}