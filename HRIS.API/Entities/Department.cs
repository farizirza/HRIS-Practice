namespace HRIS.API.Entities;

public class Department
{
    public Department(Guid id, string code, string name, Guid? parentDepartmentId)
    {
        Id = id;
        Code = code;
        Name = name;
        ParentDepartmentId = parentDepartmentId;
    }

    public Department() { }

    public Guid Id { get; set; }
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    // Foreign Keys
    public Guid? ParentDepartmentId { get; set; }
    
    // Relationships
    public Department? ParentDepartment { get; set; }
    public ICollection<Department>? SubDepartments { get; set; } = [];
    public ICollection<Employee> Employees { get; set; } = [];
}