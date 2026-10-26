using HRIS.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace HRIS.API.Data.Repositories;

public class DepartmentRepository(HrisDbContext context) : Repository<Department>(context), IDepartmentRepository
{
    public async Task<bool> CodeExistsAsync(string code, CancellationToken ct = default)
        => await Context.Departments.AnyAsync(d => d.Code == code, ct);
}