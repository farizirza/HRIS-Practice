using HRIS.API.Data;
using HRIS.API.Data.Repositories;
using HRIS.API.Entities;
using HRIS.API.Models;
using Microsoft.EntityFrameworkCore;

namespace HRIS.API.Services;

public class EfEmployeeService(IUnitOfWork uow, TimeProvider timeProvider) : IEmployeeService
{
    public async Task<IReadOnlyCollection<EmployeeResponse>> GetAllAsync(CancellationToken ct = default)
        => await uow.Employees.GetResponsesAsync(ct);

    public Task<EmployeeResponse?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => uow.Employees.GetResponseAsync(id, ct);

    public async Task<Guid> CreateAsync(CreateEmployeeRequest request, CancellationToken ct = default)
    {
        var employee = request.ToEntity(
            timeProvider.GetUtcNow(),
            EmployeeStatus.Active
            );
        
        await uow.Employees.AddAsync(employee, ct);
        await uow.SaveChangesAsync(ct);
        return employee.Id;
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateEmployeeRequest request, CancellationToken ct = default)
    {
        var employee = await uow.Employees.GetByIdAsync(id, ct);
        if (employee is null)
            return false;
        
        employee.ApplyUpdateEntity(request);
        await uow.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var employee = await uow.Employees.GetByIdAsync(id, ct);
        if (employee is null)
            return false;
        
        uow.Employees.Remove(employee);
        await uow.SaveChangesAsync(ct);
        return true;
    }

    public async Task<TerminateResult> TerminateAsync(Guid id, CancellationToken ct = default)
    {
        var employee = await uow.Employees.GetByIdAsync(id, ct);
        if (employee is null) 
            return TerminateResult.NotFound;
        if (employee.Status is EmployeeStatus.Terminated) 
            return TerminateResult.AlreadyTerminated;
        
        employee.Status = EmployeeStatus.Terminated;
        employee.TerminatedAt = DateOnly.FromDateTime(timeProvider.GetUtcNow().UtcDateTime);
        
        var directReports = await uow.Employees.GetDirectReportsAsync(id, ct);
        foreach (var report in directReports)
        {
            report.ManagerId = employee.ManagerId;
        }
        
        await uow.SaveChangesAsync(ct);
        return TerminateResult.Terminated;
    }
}