using HRIS.API.Common.Exceptions;
using HRIS.API.Data.Repositories;
using MediatR;

namespace HRIS.API.Features.Employees.Commands.AssignManager;

/// <summary>Sets or clears (ManagerId = null) the direct manager of an employee.</summary>
public record AssignManagerCommand(Guid EmployeeId, Guid? ManagerId) : IRequest;

public class AssignManagerCommandHandler(IUnitOfWork uow, ILogger<AssignManagerCommandHandler> logger)
    : IRequestHandler<AssignManagerCommand>
{
    public async Task Handle(AssignManagerCommand command, CancellationToken ct)
    {
        var employee = await uow.Employees.GetByIdAsync(command.EmployeeId, ct)
                       ?? throw new NotFoundException("Employee", command.EmployeeId);

        var oldManagerId = employee.ManagerId;
        employee.ManagerId = command.ManagerId;
        await uow.SaveChangesAsync(ct);

        logger.LogInformation("Manager of employee {EmployeeId} changed from {OldManagerId} to {ManagerId}",
            employee.Id, oldManagerId, employee.ManagerId);
    }
}