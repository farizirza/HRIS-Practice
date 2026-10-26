using HRIS.API.Entities;

namespace HRIS.API.Data.Repositories;

public interface IDepartmentRepository : IRepository<Department>
{
    Task<bool> CodeExistsAsync(string code, CancellationToken ct = default);
}