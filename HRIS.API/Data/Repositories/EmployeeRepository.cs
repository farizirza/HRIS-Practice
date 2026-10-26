using HRIS.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace HRIS.API.Data.Repositories;

public class EmployeeRepository(HrisDbContext context) : Repository<Employee>(context), IEmployeeRepository
{
    // Tracked on purpose: the caller changes ManagerId
    public async Task<IReadOnlyCollection<Employee>> GetDirectReportsAsync(Guid managerId, CancellationToken ct = default)
        => await Context.Employees.Where(e => e.ManagerId == managerId).ToListAsync(ct);
}