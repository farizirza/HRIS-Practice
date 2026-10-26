namespace HRIS.API.Data.Repositories;

public interface IUnitOfWork
{
    IEmployeeRepository Employees { get; }
    IDepartmentRepository Departments { get; }
    IPositionRepository Positions { get; }
    
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}