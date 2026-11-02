namespace HRIS.API.Entities;

public class User
{
    public Guid Id { get; set; }
    public string Email { get; set; } = default!;           // unique, normalized to lower case
    public string PasswordHash { get; set; } = default!;    // never the password itself
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public ICollection<UserRole> UserRoles { get; set; } = [];
}