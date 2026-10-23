using System.Collections.Concurrent;
using HRIS.API.Entities;

namespace HRIS.API.Services;

public class InMemoryEmployeeService : IEmployeeService
{
    private readonly ConcurrentDictionary<Guid, Employee> _employees = new();
    private readonly TimeProvider _timeProvider;

    public InMemoryEmployeeService(TimeProvider timeProvider)
    {
        _timeProvider = timeProvider;
    }

    public Task<IReadOnlyCollection<Employee>> GetAllAsync(CancellationToken ct = default)
    {
        IReadOnlyCollection<Employee> result = _employees.Values.OrderBy(e => e.Id).ToList();
        return Task.FromResult(result);
    }

    public Task<Employee?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        _employees.TryGetValue(id, out Employee? employee);
        return Task.FromResult(employee);
    }

    public Task<Employee> CreateAsync(Employee employee, CancellationToken ct = default)
    {
        employee.Id = Guid.CreateVersion7();
        employee.CreatedAt = _timeProvider.GetUtcNow();
        _employees.TryAdd(employee.Id, employee);
        return Task.FromResult(employee);
    }

    public Task<bool> UpdateAsync(Guid id, Employee employee, CancellationToken ct = default)
    {
        if (!_employees.TryGetValue(id, out Employee? existing))
        {
            return Task.FromResult(false);
        }
        
        existing.EmployeeNumber = employee.EmployeeNumber;
        existing.FullName = employee.FullName;
        existing.Email = employee.Email;
        existing.HireDate = employee.HireDate;
        
        return Task.FromResult(true);
    }

    public Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
        => Task.FromResult(_employees.TryRemove(id, out _));
}