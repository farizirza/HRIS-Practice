using HRIS.API.Data.Repositories;
using MediatR;

namespace HRIS.API.Features.Employees.Commands.DeleteEmployee;

public record DeleteEmployeeCommand(Guid Id) : IRequest<bool>;

public class DeleteEmployeeCommandHandler(IUnitOfWork uow) : IRequestHandler<DeleteEmployeeCommand, bool>
{
    public async Task<bool> Handle(DeleteEmployeeCommand command, CancellationToken ct)
    {
        var employee = await uow.Employees.GetByIdAsync(command.Id, ct);
        if (employee is null)
        {
            return false;
        }

        uow.Employees.Remove(employee);
        await uow.SaveChangesAsync(ct);   // fails while the employee still manages someone (Restrict)
        return true;
    }
}