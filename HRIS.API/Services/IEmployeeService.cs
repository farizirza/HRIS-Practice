using HRIS.API.Entities;
using HRIS.API.Models;

namespace HRIS.API.Services;

public interface IEmployeeService
{
    Task<IReadOnlyCollection<EmployeeResponse>> GetAllAsync(CancellationToken ct = default);
    Task<EmployeeResponse?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Guid> CreateAsync(CreateEmployeeRequest request, CancellationToken ct = default);
    Task<bool> UpdateAsync(Guid id, UpdateEmployeeRequest request, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
    Task<TerminateResult> TerminateAsync(Guid id, CancellationToken ct = default);
}

public enum TerminateResult
{
    Terminated,
    NotFound, 
    AlreadyTerminated 
}