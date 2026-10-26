using HRIS.API.Data.Repositories;
using HRIS.API.Models;
using MediatR;

namespace HRIS.API.Features.Employees.Commands.UpdateEmployee;

public record UpdateEmployeeCommand(Guid Id, UpdateEmployeeRequest Request) : IRequest<bool>;

public class UpdateEmployeeCommandHandler(IUnitOfWork uow) : IRequestHandler<UpdateEmployeeCommand, bool>
{
    public async Task<bool> Handle(UpdateEmployeeCommand command, CancellationToken ct)
    {
        var employee = await uow.Employees.GetByIdAsync(command.Id, ct);
        if (employee is null)
        {
            return false;
        }

        employee.ApplyUpdateEntity(command.Request);
        await uow.SaveChangesAsync(ct);
        return true;
    }
}