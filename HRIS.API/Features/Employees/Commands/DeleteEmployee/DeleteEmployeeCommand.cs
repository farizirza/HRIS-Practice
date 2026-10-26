using HRIS.API.Common.Exceptions;
using HRIS.API.Data.Repositories;
using MediatR;

namespace HRIS.API.Features.Employees.Commands.DeleteEmployee;

public record DeleteEmployeeCommand(Guid Id) : IRequest;

public class DeleteEmployeeCommandHandler(IUnitOfWork uow) : IRequestHandler<DeleteEmployeeCommand>
{
    public async Task Handle(DeleteEmployeeCommand command, CancellationToken ct)
    {
        var employee = await uow.Employees.GetByIdAsync(command.Id, ct)
                       ?? throw new NotFoundException("Employee", command.Id);

        // The database refuses to delete a manager (Restrict). Check first, so the client gets a clear 409 and not a 500.
        // A category exception can be thrown directly when no specific type is needed.
        if ((await uow.Employees.GetDirectReportsAsync(employee.Id, ct)).Count > 0)
        {
            throw new ConflictException($"Employee '{employee.Id}' still manages other employees. Reassign them first.");
        }

        uow.Employees.Remove(employee);
        await uow.SaveChangesAsync(ct);
    }
}