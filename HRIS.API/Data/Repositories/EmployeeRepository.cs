using HRIS.API.Entities;
using HRIS.API.Models;
using Microsoft.EntityFrameworkCore;

namespace HRIS.API.Data.Repositories;

public class EmployeeRepository(HrisDbContext context) : Repository<Employee>(context), IEmployeeRepository
{
    public async Task<IReadOnlyCollection<EmployeeResponse>> GetResponsesAsync(CancellationToken ct = default)
        => await Context.Employees
            .AsNoTracking()
            .OrderBy(e => e.FullName)
            .Select(e => e.ToResponse(
                e.Department.Name,
                e.Position.Title,
                e.Manager != null ? e.Manager.FullName : null))
            .ToListAsync(ct);

    public async Task<EmployeeResponse?> GetResponseAsync(Guid id, CancellationToken ct = default)
        => await Context.Employees
            .AsNoTracking()
            .Where(e => e.Id == id)
            .Select(e => e.ToResponse(
                e.Department.Name,
                e.Position.Title,
                e.Manager != null ? e.Manager.FullName : null))
            .FirstOrDefaultAsync(ct);

    public async Task<IReadOnlyCollection<Employee>> GetDirectReportsAsync(Guid managerId,
        CancellationToken ct = default)
        => await Context.Employees.Where(e => e.ManagerId == managerId).ToListAsync(ct);
}