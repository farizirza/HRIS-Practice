namespace HRIS.API.Models;

public record CreateEmployeeRequest(
    string EmployeeNumber, 
    string FullName, 
    string Email,
    DateOnly HireDate);

public record UpdateEmployeeRequest(
    string EmployeeNumber,
    string FullName,
    string Email,
    DateOnly HireDate
);

public record EmployeeResponse(
    Guid EmployeeId,
    string FullName,
    string Email,
    DateOnly HireDate,
    DateTimeOffset CreatedAt
);