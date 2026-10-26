using HRIS.API.Data.Repositories;
using HRIS.API.Entities;
using MediatR;

namespace HRIS.API.Features.Employees.Commands.TerminateEmployee;

public enum TerminateResult { Terminated, NotFound, AlreadyTerminated }

public record TerminateEmployeeCommand(Guid EmployeeId) : IRequest<TerminateResult>;

public class TerminateEmployeeCommandHandler(IUnitOfWork uow, TimeProvider timeProvider, ILogger<TerminateEmployeeCommandHandler> logger)
    : IRequestHandler<TerminateEmployeeCommand, TerminateResult>
{
    public async Task<TerminateResult> Handle(TerminateEmployeeCommand command, CancellationToken ct)
    {
        var employee = await uow.Employees.GetByIdAsync(command.EmployeeId, ct);
        if (employee is null)
        {
            return TerminateResult.NotFound;
        }

        if (employee.Status is EmployeeStatus.Terminated)
        {
            return TerminateResult.AlreadyTerminated;
        }

        employee.Status = EmployeeStatus.Terminated;
        employee.TerminatedAt = DateOnly.FromDateTime(timeProvider.GetUtcNow().UtcDateTime);

        var directReports = await uow.Employees.GetDirectReportsAsync(employee.Id, ct);
        foreach (var report in directReports)
        {
            report.ManagerId = employee.ManagerId;
        }

        await uow.SaveChangesAsync(ct);   // one unit of work, as in Module 5, now inside a handler

        logger.LogInformation("Employee {EmployeeId} terminated; {ReassignedCount} direct reports moved to manager {ManagerId}",
            employee.Id, directReports.Count, employee.ManagerId);
        return TerminateResult.Terminated;
    }
}