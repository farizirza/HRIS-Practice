using HRIS.API.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRIS.API.Data.Configurations;

public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.EmployeeNumber)
            .HasMaxLength(20)
            .IsRequired();
        builder.Property(e => e.FullName)
            .HasMaxLength(100)
            .IsRequired();
        builder.Property(e => e.Email)
            .HasMaxLength(256)
            .IsRequired();
        builder.Property(e => e.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            // .HasDefaultValue(EmployeeStatus.Active)
            .IsRequired();
        
        builder.HasIndex(e => e.EmployeeNumber)
            .IsUnique();
        builder.HasIndex(e => e.Email)
            .IsUnique();

        builder.HasOne(e => e.Department)
            .WithMany(d => d.Employees)
            .HasForeignKey(e => e.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(e => e.Position)
            .WithMany(p => p.Employees)
            .HasForeignKey(e => e.PositionId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(e => e.Manager)
            .WithMany(m => m.DirectReports)
            .HasForeignKey(e => e.ManagerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasData(SeedEmployees());
    }

    private static readonly DateTimeOffset SeededAt = new(2026, 1, 5, 0, 0, 0, TimeSpan.Zero);

    private static Employee Seed(Guid id, string number, string fullName, string email, DateOnly hireDate,
        Guid departmentId, Guid positionId, Guid? managerId) =>
        new(id, number, fullName, email, hireDate, SeededAt, EmployeeStatus.Active, null,
            departmentId, positionId, managerId);

    private static IEnumerable<Employee> SeedEmployees()
    {
        // A manager comes before the people who report to him
        yield return Seed(DemoSeedIds.Dewi, "EMP-1001", "Dewi Lestari", "dewi@demo.hris.test",
            new DateOnly(2020, 1, 6), DemoSeedIds.Tech, DemoSeedIds.DepartmentHead, null);
        yield return Seed(DemoSeedIds.Budi, "EMP-1002", "Budi Hartono", "budi@demo.hris.test",
            new DateOnly(2021, 3, 1), DemoSeedIds.Tech, DemoSeedIds.Supervisor, DemoSeedIds.Dewi);
        yield return Seed(DemoSeedIds.Siti, "EMP-1003", "Siti Aminah", "siti@demo.hris.test",
            new DateOnly(2022, 6, 15), DemoSeedIds.TechBackend, DemoSeedIds.Staff, DemoSeedIds.Budi);
        yield return Seed(DemoSeedIds.Andi, "EMP-1004", "Andi Pratama", "andi@demo.hris.test",
            new DateOnly(2022, 9, 1), DemoSeedIds.TechBackend, DemoSeedIds.Staff, DemoSeedIds.Budi);

        for (var n = 5; n <= 14; n++)
        {
            yield return Seed(DemoSeedIds.StaffMember(n), $"EMP-10{n:D2}", $"Staff {n}", $"staff{n}@demo.hris.test",
                new DateOnly(2024, 1, n), DemoSeedIds.People, DemoSeedIds.Staff, null);
        }
    }
}