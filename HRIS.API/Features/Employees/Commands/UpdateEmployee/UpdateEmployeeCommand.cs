using HRIS.API.Common.Exceptions;
using HRIS.API.Data;
using HRIS.API.Data.Repositories;
using HRIS.API.Extensions;
using HRIS.API.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRIS.API.Features.Employees.Commands.UpdateEmployee;

public record UpdateEmployeeCommand(Guid Id, UpdateEmployeeRequest Request) : IRequest;

public class UpdateEmployeeCommandHandler(IUnitOfWork uow) : IRequestHandler<UpdateEmployeeCommand>
{
    public async Task Handle(UpdateEmployeeCommand command, CancellationToken ct)
    {
        var employee = await uow.Employees.GetByIdAsync(command.Id, ct)
                       ?? throw new NotFoundException("Employee", command.Id);

        var request = command.Request with { Email = command.Request.Email.NormalizeEmail() };
        employee.ApplyUpdateEntity(request);

        try
        {
            await uow.SaveChangesAsync(ct);
        }
        catch (DbUpdateException ex) when (ex.IsUniqueConstraintViolation())
        {
            throw new DuplicateEmployeeException("Employee number or email is already used.", ex);
        }
    }
}