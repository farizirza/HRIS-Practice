namespace HRIS.API.Models;

public record CreateDepartmentRequest(
    string Code, 
    string Name, 
    Guid? ParentDepartmentId);

public record DepartmentResponse(
    Guid Id, 
    string Code, 
    string Name, 
    Guid? ParentDepartmentId, 
    string? ParentDepartmentName);