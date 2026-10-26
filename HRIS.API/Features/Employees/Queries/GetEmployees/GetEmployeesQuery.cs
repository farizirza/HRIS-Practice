using HRIS.API.Common.Paging;
using HRIS.API.Data;
using HRIS.API.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRIS.API.Features.Employees.Queries.GetEmployees;

public record GetEmployeesQuery(
    string? Search, Guid? DepartmentId, EmployeeStatus? Status,
    string? SortBy, bool Desc = false,
    int Page = 1, int PageSize = 10) : IRequest<PagedResult<EmployeeListDto>>;

/// <summary>Directory row: only the fields a list needs.</summary>
public record EmployeeListDto(
    Guid Id, string EmployeeNumber, string FullName, string DepartmentName,
    string PositionTitle, string? ManagerName, EmployeeStatus Status);

public class GetEmployeesQueryHandler(HrisDbContext db) : IRequestHandler<GetEmployeesQuery, PagedResult<EmployeeListDto>>
{
    public async Task<PagedResult<EmployeeListDto>> Handle(GetEmployeesQuery query, CancellationToken ct)
    {
        IQueryable<Employee> employees = db.Employees.AsNoTracking();

        // 1. Filter: add a Where only when the parameter has a value
        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            employees = employees.Where(e => e.FullName.Contains(query.Search) || e.EmployeeNumber.Contains(query.Search));
        }

        if (query.DepartmentId is not null)
        {
            employees = employees.Where(e => e.DepartmentId == query.DepartmentId);
        }

        var status = query.Status ?? EmployeeStatus.Active;   // default: the directory shows active employees only
        employees = employees.Where(e => e.Status == status);

        // 2. Sort: a whitelist, plus a unique tie-breaker so pages are stable
        employees = query.SortBy?.ToLowerInvariant() switch
        {
            "hiredate" => query.Desc
                ? employees.OrderByDescending(e => e.HireDate).ThenBy(e => e.EmployeeNumber)
                : employees.OrderBy(e => e.HireDate).ThenBy(e => e.EmployeeNumber),
            "number" => query.Desc
                ? employees.OrderByDescending(e => e.EmployeeNumber)
                : employees.OrderBy(e => e.EmployeeNumber),
            _ => query.Desc
                ? employees.OrderByDescending(e => e.FullName).ThenBy(e => e.EmployeeNumber)
                : employees.OrderBy(e => e.FullName).ThenBy(e => e.EmployeeNumber)
        };

        // 3. Project + 4. Page
        return await employees
            .Select(e => new EmployeeListDto(
                e.Id, e.EmployeeNumber, e.FullName, e.Department.Name, e.Position.Title,
                e.Manager != null ? e.Manager.FullName : null, e.Status))
            .ToPagedResultAsync(query.Page, query.PageSize, ct: ct);
    }
}