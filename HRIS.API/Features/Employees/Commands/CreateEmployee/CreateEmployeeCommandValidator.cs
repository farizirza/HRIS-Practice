using FluentValidation;
using HRIS.API.Data;
using Microsoft.EntityFrameworkCore;

namespace HRIS.API.Features.Employees.Commands.CreateEmployee;

public class CreateEmployeeCommandValidator : AbstractValidator<CreateEmployeeCommand>
{
    public const int MaxFutureHireDays = 90;

    public CreateEmployeeCommandValidator(HrisDbContext db, TimeProvider timeProvider)
    {
        RuleFor(x => x.Request.EmployeeNumber)
            .NotEmpty()
            .Matches(@"^EMP-\d{4}$").WithMessage("Employee number must look like EMP-0001.");

        RuleFor(x => x.Request.FullName).NotEmpty().MaximumLength(100);

        RuleFor(x => x.Request.Email).NotEmpty().EmailAddress().MaximumLength(256);

        // "Today" comes from TimeProvider, never from DateTime.UtcNow: Module 8 freezes the clock in tests.
        RuleFor(x => x.Request.HireDate)
            .NotEmpty()
            .Must(date => date <= DateOnly.FromDateTime(timeProvider.GetUtcNow().UtcDateTime).AddDays(MaxFutureHireDays))
            .WithMessage($"Hire date cannot be more than {MaxFutureHireDays} days in the future.");

        // MustAsync = rule that needs I/O. The validator shares the scoped DbContext with the handler.
        RuleFor(x => x.Request.DepartmentId)
            .MustAsync(async (id, ct) => await db.Departments.AnyAsync(d => d.Id == id, ct))
            .WithMessage("Department does not exist.");

        RuleFor(x => x.Request.PositionId)
            .MustAsync(async (id, ct) => await db.Positions.AnyAsync(p => p.Id == id, ct))
            .WithMessage("Position does not exist.");

        // When(): the rule only runs if a manager was sent.
        RuleFor(x => x.Request.ManagerId)
            .MustAsync(async (id, ct) => await db.Employees.AnyAsync(e => e.Id == id, ct))
            .WithMessage("Manager does not exist.")
            .When(x => x.Request.ManagerId is not null);
    }
}