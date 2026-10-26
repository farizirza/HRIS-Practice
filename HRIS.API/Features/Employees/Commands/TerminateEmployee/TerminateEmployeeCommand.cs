using HRIS.API.Common.Exceptions;
using HRIS.API.Data.Repositories;
using HRIS.API.Entities;
using MediatR;

namespace HRIS.API.Features.Employees.Commands.TerminateEmployee;

public record TerminateEmployeeCommand(Guid EmployeeId) : IRequest;

public class TerminateEmployeeCommandHandler(IUnitOfWork uow, TimeProvider timeProvider, ILogger<TerminateEmployeeCommandHandler> logger)
    : IRequestHandler<TerminateEmployeeCommand>
{
    public async Task Handle(TerminateEmployeeCommand command, CancellationToken ct)
    {
        var employee = await uow.Employees.GetByIdAsync(command.EmployeeId, ct)
                       ?? throw new NotFoundException("Employee", command.EmployeeId);

        if (employee.Status == EmployeeStatus.Terminated)
        {
            throw new EmployeeAlreadyTerminatedException(employee.Id);
        }

        employee.Status = EmployeeStatus.Terminated;
        employee.TerminatedAt = DateOnly.FromDateTime(timeProvider.GetUtcNow().UtcDateTime);

        var directReports = await uow.Employees.GetDirectReportsAsync(employee.Id, ct);
        foreach (var report in directReports)
        {
            report.ManagerId = employee.ManagerId;
        }

        await uow.SaveChangesAsync(ct);   // one unit of work: all changes or none

        logger.LogInformation("Employee {EmployeeId} terminated; {ReassignedCount} direct reports moved to manager {ManagerId}",
            employee.Id, directReports.Count, employee.ManagerId);
    }
}