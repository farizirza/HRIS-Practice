using FluentValidation;
using HRIS.API.Common;
using HRIS.API.Data;
using Microsoft.EntityFrameworkCore;

namespace HRIS.API.Features.Employees.Commands.CreateEmployee;

public class CreateEmployeeCommandValidator : AbstractValidator<CreateEmployeeCommand>
{
    public CreateEmployeeCommandValidator(HrisDbContext db, EmployeePolicy policy)
    {
        RuleFor(x => x.Request.EmployeeNumber)
            .NotEmpty()
            .Matches(@"^EMP-\d{4}$").WithMessage("Employee number must look like EMP-0001.");

        RuleFor(x => x.Request.FullName).NotEmpty().MaximumLength(100);

        RuleFor(x => x.Request.Email).NotEmpty().EmailAddress().MaximumLength(256);

        RuleFor(x => x.Request.HireDate)
            .NotEmpty()
            .Must(policy.IsValidHireDate)
            .WithMessage($"Hire date cannot be more than {EmployeePolicy.MaxFutureHireDays} days in the future.");

        RuleFor(x => x.Request.DepartmentId)
            .MustAsync(async (id, ct) => await db.Departments.AnyAsync(d => d.Id == id, ct))
            .WithMessage("Department does not exist.");

        RuleFor(x => x.Request.PositionId)
            .MustAsync(async (id, ct) => await db.Positions.AnyAsync(p => p.Id == id, ct))
            .WithMessage("Position does not exist.");

        RuleFor(x => x.Request.ManagerId)
            .MustAsync(async (id, ct) => await db.Employees.AnyAsync(e => e.Id == id, ct))
            .WithMessage("Manager does not exist.")
            .When(x => x.Request.ManagerId is not null);
    }
}
