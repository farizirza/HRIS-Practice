using HRIS.API.Entities;

namespace HRIS.API.Models;

public static class EmployeeMappings
{
    public static EmployeeResponse ToResponse(
        this Employee employee, 
        string departmentName,
        string positionTitle,
        string? managerFullName) =>
        new(employee.Id, 
            employee.EmployeeNumber,
            employee.FullName, 
            employee.Email, 
            employee.HireDate, 
            employee.Status, 
            departmentName, 
            positionTitle, 
            managerFullName, 
            employee.CreatedAt);
    
    public static Employee ToEntity(
        this CreateEmployeeRequest request,
        DateTimeOffset createdAt,
        EmployeeStatus employeeStatus) =>
        new(Guid.CreateVersion7(),
            request.EmployeeNumber,
            request.FullName,
            request.Email,
            request.HireDate,
            createdAt,
            employeeStatus,
            null,
            request.DepartmentId,
            request.PositionId,
            request.ManagerId);

    public static void ApplyUpdateEntity(this Employee employee, UpdateEmployeeRequest request)
    {
        employee.EmployeeNumber = request.EmployeeNumber;
        employee.FullName = request.FullName;
        employee.Email = request.Email;
        employee.HireDate = request.HireDate;
        employee.DepartmentId = request.DepartmentId;
        employee.PositionId = request.PositionId;
        employee.ManagerId = request.ManagerId;
    }
}