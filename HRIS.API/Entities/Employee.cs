namespace HRIS.API.Entities;

public enum EmployeeStatus
{
    Active,
    Terminated
}

public class Employee
{
    public Guid Id { get; set; }
    public string EmployeeNumber { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public DateOnly HireDate { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public EmployeeStatus EmployeeStatus { get; set; } = EmployeeStatus.Active;
    public DateOnly? TerminatedAt { get; set; }
    // Foreign Keys
    public Guid DepartmentId { get; set; } 
    public Guid PositionId { get; set; }
    public Guid? ManagerId { get; set; }
    
    // Relationships
    public Department Department { get; set; } = null!;
    public Position Position { get; set; } = null!;
    public Employee? Manager { get; set; }
    public ICollection<Employee> DirectReports { get; set; } = [];
}