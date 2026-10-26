using HRIS.API.Entities;
using HRIS.API.Models;
using Microsoft.EntityFrameworkCore;

namespace HRIS.API.Data.Repositories;

public class DepartmentRepository(HrisDbContext context) : Repository<Department>(context), IDepartmentRepository
{
    public async Task<IReadOnlyCollection<DepartmentResponse>> GetResponsesAsync(CancellationToken ct = default)
        => await Context.Departments
            .AsNoTracking()
            .OrderBy(d => d.Code)
            .Select(d => d.ToResponse(
                d.ParentDepartment != null 
                    ? d.ParentDepartment.Name 
                    : null))
            .ToListAsync(ct);
}