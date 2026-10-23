namespace HRIS.API.Entities;

public class Position
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public int Level { get; set; }
    
    // Relationships
    public ICollection<Employee> Employees { get; set; } = [];
}