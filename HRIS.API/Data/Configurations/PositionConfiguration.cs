using HRIS.API.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRIS.API.Data.Configurations;

public class PositionConfiguration : IEntityTypeConfiguration<Position>
{
    public void Configure(EntityTypeBuilder<Position> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Title)
            .HasMaxLength(100)
            .IsRequired();
        builder.Property(p => p.Level)
            .IsRequired();
        
        builder.HasIndex(p => p.Title)
            .IsUnique();
        
        builder.HasData(
            new Position {Id = Guid.Parse("01a166bf-2518-743a-bc79-972df6de3ecc"), Title = "Staff", Level = 2},
            new Position {Id = Guid.Parse("01a166bf-2518-743a-bc79-98d145939083"), Title = "Supervisor", Level = 5},
            new Position {Id = Guid.Parse("01a166bf-2518-743a-bc79-9e1819223d9f"), Title = "Department Head", Level = 8}
        );
    }
}