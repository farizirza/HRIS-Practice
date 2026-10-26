using HRIS.API.Data.Repositories;
using HRIS.API.Models;
using MediatR;

namespace HRIS.API.Features.Departments.Commands.CreateDepartment;

public record CreateDepartmentCommand(CreateDepartmentRequest Request) : IRequest<Guid>;

public class CreateDepartmentCommandHandler(IUnitOfWork uow) : IRequestHandler<CreateDepartmentCommand, Guid>
{
    public async Task<Guid> Handle(CreateDepartmentCommand command, CancellationToken ct)
    {
        var department = command.Request.ToEntity();

        await uow.Departments.AddAsync(department, ct);
        await uow.SaveChangesAsync(ct);
        return department.Id;
    }
}