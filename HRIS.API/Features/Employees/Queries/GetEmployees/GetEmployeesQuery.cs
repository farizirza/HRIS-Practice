using HRIS.API.Data;
using HRIS.API.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRIS.API.Features.Employees.Queries.GetEmployees;

public record GetEmployeesQuery : IRequest<IReadOnlyCollection<EmployeeResponse>>;

public class GetEmployeesQueryHandler(HrisDbContext db)
    : IRequestHandler<GetEmployeesQuery, IReadOnlyCollection<EmployeeResponse>>
{
    public async Task<IReadOnlyCollection<EmployeeResponse>> Handle(GetEmployeesQuery query, CancellationToken ct)
        => await db.Employees
            .AsNoTracking()
            .OrderBy(e => e.FullName)
            .Select(e => e.ToResponse(
                e.Department.Name,
                e.Position.Title,
                e.Manager != null ? e.Manager.FullName : null))
            .ToListAsync(ct);
}