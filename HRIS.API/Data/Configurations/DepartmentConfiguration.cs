using HRIS.API.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRIS.API.Data.Configurations;

public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
{
    public void Configure(EntityTypeBuilder<Department> builder)
    {
        builder.HasKey(d => d.Id);
        builder.Property(d => d.Code)
            .HasMaxLength(10)
            .IsRequired();
        builder.Property(d => d.Name)
            .HasMaxLength(100)
            .IsRequired();
        
        builder.HasIndex(d => d.Code)
            .IsUnique();
        
        // Relations
        builder.HasOne(d => d.ParentDepartment)
            .WithMany(d => d.SubDepartments)
            .HasForeignKey(d => d.ParentDepartmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasData(
            new Department(DemoSeedIds.Tech, "TECH", "Technology", null),
            new Department(DemoSeedIds.TechBackend, "TECH-BE", "Backend", DemoSeedIds.Tech),
            new Department(DemoSeedIds.People, "PEOPLE", "People Operations", null));
    }
}