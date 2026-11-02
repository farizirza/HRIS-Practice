using HRIS.API.Common.Security;
using HRIS.API.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRIS.API.Data.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.HasKey(r => r.Id);
        builder.Property(r => r.Name).HasMaxLength(50).IsRequired();
        builder.HasIndex(r => r.Name).IsUnique();

        // Reference data: the four roles exist after Update-Database, with the same Ids on every machine.
        builder.HasData(
            new Role { Id = RoleIds.Admin, Name = Roles.Admin },
            new Role { Id = RoleIds.Hr, Name = Roles.Hr },
            new Role { Id = RoleIds.Manager, Name = Roles.Manager },
            new Role { Id = RoleIds.Employee, Name = Roles.Employee });
    }
}