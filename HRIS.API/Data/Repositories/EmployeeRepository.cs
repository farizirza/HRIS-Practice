using HRIS.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace HRIS.API.Data.Repositories;

public class EmployeeRepository(HrisDbContext context) : Repository<Employee>(context), IEmployeeRepository
{
    // Tracked on purpose: the caller changes ManagerId
    public async Task<IReadOnlyCollection<Employee>> GetDirectReportsAsync(Guid managerId, CancellationToken ct = default)
        => await Context.Employees.Where(e => e.ManagerId == managerId).ToListAsync(ct);
    public async Task<bool> EmployeeNumberExistsAsync(string employeeNumber, CancellationToken ct = default)
        => await Context.Employees.AnyAsync(e => e.EmployeeNumber == employeeNumber, ct);

    public async Task<bool> EmailExistsAsync(string email, CancellationToken ct = default)
        => await Context.Employees.AnyAsync(e => e.Email == email, ct);
}