using HRIS.API.Data;
using HRIS.API.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRIS.API.Features.Departments.Queries.GetDepartmentTree;

public record SubDepartmentDto(Guid Id, string Code, string Name, int Headcount);
public record DepartmentTreeDto(Guid Id, string Code, string Name, int Headcount, List<SubDepartmentDto> SubDepartments);

public record GetDepartmentTreeQuery : IRequest<IReadOnlyCollection<DepartmentTreeDto>>;

/// <summary>Projection over a self-referencing relation: root departments with their direct sub-departments.</summary>
public class GetDepartmentTreeQueryHandler(HrisDbContext db)
    : IRequestHandler<GetDepartmentTreeQuery, IReadOnlyCollection<DepartmentTreeDto>>
{
    public async Task<IReadOnlyCollection<DepartmentTreeDto>> Handle(GetDepartmentTreeQuery query, CancellationToken ct)
        => await db.Departments
            .AsNoTracking()
            .Where(d => d.ParentDepartmentId == null)               // roots only
            .OrderBy(d => d.Name)
            .Select(d => new DepartmentTreeDto(
                d.Id, d.Code, d.Name,
                d.Employees.Count(e => e.Status == EmployeeStatus.Active),
                d.SubDepartments
                    .OrderBy(s => s.Name)
                    .Select(s => new SubDepartmentDto(s.Id, s.Code, s.Name,
                        s.Employees.Count(e => e.Status == EmployeeStatus.Active)))
                    .ToList()))
            .ToListAsync(ct);
}