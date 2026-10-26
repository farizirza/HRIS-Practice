using HRIS.API.Common.Exceptions;
using HRIS.API.Data;
using HRIS.API.Data.Repositories;
using HRIS.API.Entities;
using HRIS.API.Extensions;
using HRIS.API.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRIS.API.Features.Employees.Commands.CreateEmployee;

public record CreateEmployeeCommand(CreateEmployeeRequest Request) : IRequest<Guid>;

public class CreateEmployeeCommandHandler(IUnitOfWork uow, TimeProvider timeProvider, ILogger<CreateEmployeeCommandHandler> logger)
    : IRequestHandler<CreateEmployeeCommand, Guid>
{
    public async Task<Guid> Handle(CreateEmployeeCommand command, CancellationToken ct)
    {
        // Normalization (Lesson 2, Step 6): after validation, before the checks and the save.
        var request = command.Request with { Email = command.Request.Email.NormalizeEmail() };

        // Business rules that need the database. The shape of the input was already checked by the validator.
        if (await uow.Employees.EmployeeNumberExistsAsync(request.EmployeeNumber, ct))
        {
            throw new DuplicateEmployeeException($"Employee number '{request.EmployeeNumber}' is already used.");
        }

        if (await uow.Employees.EmailExistsAsync(request.Email, ct))
        {
            throw new DuplicateEmployeeException("The email is already used by another employee.");   // no e-mail in the message: messages are logged
        }

        // The handler sets the default status (the entity has no initializer). CreatedAt comes from TimeProvider.
        var employee = request.ToEntity(timeProvider.GetUtcNow(), EmployeeStatus.Active);

        await uow.Employees.AddAsync(employee, ct);

        try
        {
            await uow.SaveChangesAsync(ct);
        }
        // Two HR users onboard the same person at the same moment: both checks above passed, the unique index decides.
        // Exception FILTER: only a unique-index violation is translated. Any other DbUpdateException is not caught at all
        // and travels up unchanged to GlobalExceptionHandler (500).
        catch (DbUpdateException ex) when (ex.IsUniqueConstraintViolation())
        {
            throw new DuplicateEmployeeException("Employee number or email is already used.", ex);   // keep the cause as InnerException
        }

        logger.LogInformation("Employee {EmployeeId} created", employee.Id);   // the id only: no name or email in logs
        return employee.Id;
    }
}