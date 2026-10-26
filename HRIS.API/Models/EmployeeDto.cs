using HRIS.API.Entities;

namespace HRIS.API.Models;

public record CreateEmployeeRequest(
    string EmployeeNumber, 
    string FullName, 
    string Email,
    DateOnly HireDate,
    Guid DepartmentId,
    Guid PositionId,
    Guid? ManagerId);

public record UpdateEmployeeRequest(
    string EmployeeNumber,
    string FullName,
    string Email,
    DateOnly HireDate,
    Guid DepartmentId,
    Guid PositionId,
    Guid? ManagerId);

public record EmployeeResponse(
    Guid Id,
    string EmployeeNumber,
    string FullName,
    string Email,
    DateOnly HireDate,
    EmployeeStatus Status, 
    string DepartmentName,
    string PositionTitle,
    string? ManagerName,
    DateTimeOffset CreatedAt
);

public record AssignManagerRequest(Guid? ManagerId);   // null = no manager