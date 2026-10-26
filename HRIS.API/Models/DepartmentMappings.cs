using HRIS.API.Entities;

namespace HRIS.API.Models;

public static class DepartmentMappings
{
    public static DepartmentResponse ToResponse(
        this Department department,
        string? departmentName) => 
        new(department.Id, 
            department.Code,
            department.Name,
            department.ParentDepartmentId,
            departmentName);
    
    public static Department ToEntity(this CreateDepartmentRequest request) =>
        new(Guid.CreateVersion7(),
            request.Code, 
            request.Name,
            request.ParentDepartmentId);
}