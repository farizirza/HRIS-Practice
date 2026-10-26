namespace HRIS.API.Data.Repositories;

public class UnitOfWork(
    HrisDbContext context,
    IEmployeeRepository employees,
    IDepartmentRepository departments,
    IPositionRepository positions
) : IUnitOfWork
{
    public IEmployeeRepository Employees { get; } = employees;
    public IDepartmentRepository Departments { get; } = departments;
    public IPositionRepository Positions { get; } = positions;

    public Task<int> SaveChangesAsync(CancellationToken ct = default)
        => context.SaveChangesAsync(ct);
}