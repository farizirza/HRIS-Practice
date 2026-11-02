namespace HRIS.API.Entities;

public class UserRole   // join table: many-to-many between User and Role
{
    public Guid UserId { get; set; }
    public User User { get; set; } = default!;
    public Guid RoleId { get; set; }
    public Role Role { get; set; } = default!;
}