using HRIS.API.Entities;

namespace HRIS.API.Data.Repositories;

public interface IEmployeeRepository : IRepository<Employee>
{
    // Domain-specific query that the generic repository cannot express by name.
    // Reads for the API (lists, DTOs) live in query handlers since Module 6.
    Task<IReadOnlyCollection<Employee>> GetDirectReportsAsync(Guid managerId, CancellationToken ct = default);
}