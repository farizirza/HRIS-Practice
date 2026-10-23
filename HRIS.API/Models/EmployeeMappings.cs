using HRIS.API.Entities;

namespace HRIS.API.Models;

public static class EmployeeMappings
{
    public static EmployeeResponse ToResponse(this Employee employee) =>
        new(employee.Id, employee.FullName, employee.Email, employee.HireDate, employee.CreatedAt);
}