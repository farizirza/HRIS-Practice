namespace HRIS.API.Data.Configurations;

// Fixed ids for the demo data. HasData needs the same values on every build.
public static class DemoSeedIds
{
    // Positions seeded in Module 4 (PositionConfiguration)
    public static readonly Guid Staff = Guid.Parse("01a166be-ee55-747c-b090-69f7572f26e8");
    public static readonly Guid Supervisor = Guid.Parse("01a166be-ee55-747c-b090-6c3b9bd9cd5c");
    public static readonly Guid DepartmentHead = Guid.Parse("01a166be-ee55-747c-b090-72cbd94cb9f2");

    // Departments
    public static readonly Guid Tech = Guid.Parse("0199f200-0000-7000-8000-0000000000d1");
    public static readonly Guid TechBackend = Guid.Parse("0199f200-0000-7000-8000-0000000000d2");
    public static readonly Guid People = Guid.Parse("0199f200-0000-7000-8000-0000000000d3");

    // Employees
    public static readonly Guid Dewi = Guid.Parse("0199f200-0000-7000-8000-0000000000e1");
    public static readonly Guid Budi = Guid.Parse("0199f200-0000-7000-8000-0000000000e2");
    public static readonly Guid Siti = Guid.Parse("0199f200-0000-7000-8000-0000000000e3");
    public static readonly Guid Andi = Guid.Parse("0199f200-0000-7000-8000-0000000000e4");

    // Staff 5 to Staff 14: ...000000000105 to ...000000000114
    public static Guid StaffMember(int number) =>
        Guid.Parse($"0199f200-0000-7000-8000-0000000001{number:D2}");
}
