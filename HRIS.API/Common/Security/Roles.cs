namespace HRIS.API.Common.Security;

public static class Roles
{
    public const string Admin = "Admin";
    public const string Hr = "HR";
    public const string Manager = "Manager";
    public const string Employee = "Employee";

    public const string HrOrAdmin = Admin + "," + Hr;   // [Authorize(Roles = Roles.HrOrAdmin)] means Admin OR HR
}

public static class RoleIds
{
    // Fixed values for HasData. Never use Guid.NewGuid() or Guid.CreateVersion7() inside HasData:
    // a new value on every build makes EF think the model changed (PendingModelChangesWarning).
    public static readonly Guid Admin = Guid.Parse("0199f000-0000-7000-8000-0000000000a1");
    public static readonly Guid Hr = Guid.Parse("0199f000-0000-7000-8000-0000000000a2");
    public static readonly Guid Manager = Guid.Parse("0199f000-0000-7000-8000-0000000000a3");
    public static readonly Guid Employee = Guid.Parse("0199f000-0000-7000-8000-0000000000a4");
}