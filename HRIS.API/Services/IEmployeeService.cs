using HRIS.API.Entities;

namespace HRIS.API.Services;

public interface IEmployeeService
{
    Task<IReadOnlyCollection<Employee>> GetAllAsync(CancellationToken ct = default);
    Task<Employee?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Employee> CreateAsync(Employee employee, CancellationToken ct = default);
    Task<bool> UpdateAsync(Guid id, Employee employee, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
}