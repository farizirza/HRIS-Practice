using HRIS.API.Data.Repositories;
using HRIS.API.Entities;
using HRIS.API.Models;
using MediatR;

namespace HRIS.API.Features.Employees.Commands.CreateEmployee;

public record CreateEmployeeCommand(CreateEmployeeRequest Request) : IRequest<Guid>;

public class CreateEmployeeCommandHandler(IUnitOfWork uow, TimeProvider timeProvider, ILogger<CreateEmployeeCommandHandler> logger)
    : IRequestHandler<CreateEmployeeCommand, Guid>
{
    public async Task<Guid> Handle(CreateEmployeeCommand command, CancellationToken ct)
    {
        // CreatedAt comes from the server clock; the request has no CreatedAt
        var employee = command.Request.ToEntity(
            timeProvider.GetUtcNow(),
            EmployeeStatus.Active);

        await uow.Employees.AddAsync(employee, ct);
        await uow.SaveChangesAsync(ct);

        logger.LogInformation("Employee {EmployeeId} created", employee.Id);   // the id only: no name or email in logs
        return employee.Id;
    }
}