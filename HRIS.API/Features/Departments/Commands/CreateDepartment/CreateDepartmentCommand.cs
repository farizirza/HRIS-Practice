using HRIS.API.Common.Exceptions;
using HRIS.API.Data;
using HRIS.API.Data.Repositories;
using HRIS.API.Extensions;
using HRIS.API.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRIS.API.Features.Departments.Commands.CreateDepartment;

public record CreateDepartmentCommand(CreateDepartmentRequest Request) : IRequest<Guid>;

public class CreateDepartmentCommandHandler(IUnitOfWork uow) : IRequestHandler<CreateDepartmentCommand, Guid>
{
    public async Task<Guid> Handle(CreateDepartmentCommand command, CancellationToken ct)
    {
        // Normalization (Lesson 2, Step 6): after validation, before the check and the save.
        var request = command.Request with { Code = command.Request.Code.NormalizeCode() };

        if (await uow.Departments.CodeExistsAsync(request.Code, ct))
        {
            throw new DuplicateDepartmentException($"Department code '{request.Code}' is already used.");
        }

        var department = request.ToEntity();
        await uow.Departments.AddAsync(department, ct);

        try
        {
            await uow.SaveChangesAsync(ct);
        }
        // Two requests with the same new code at the same moment: both checks passed, the unique index decides.
        catch (DbUpdateException ex) when (ex.IsUniqueConstraintViolation())
        {
            throw new DuplicateDepartmentException("Department code is already used.", ex);   // keep the cause as InnerException
        }

        return department.Id;
    }
}