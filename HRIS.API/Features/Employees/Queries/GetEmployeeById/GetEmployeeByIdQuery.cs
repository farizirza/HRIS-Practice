using HRIS.API.Common.Exceptions;
using HRIS.API.Data;
using HRIS.API.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRIS.API.Features.Employees.Queries.GetEmployeeById;

public record GetEmployeeByIdQuery(Guid Id) : IRequest<EmployeeResponse>;

public class GetEmployeeByIdQueryHandler(HrisDbContext db) : IRequestHandler<GetEmployeeByIdQuery, EmployeeResponse>
{
    public async Task<EmployeeResponse> Handle(GetEmployeeByIdQuery query, CancellationToken ct)
        => await db.Employees
               .AsNoTracking()
               .Where(e => e.Id == query.Id)
               .Select(e => e.ToResponse(
                   e.Department.Name,
                   e.Position.Title,
                   e.Manager != null ? e.Manager.FullName : null))
               .FirstOrDefaultAsync(ct)
           ?? throw new NotFoundException("Employee", query.Id);   // the exception handler turns it into 404
}