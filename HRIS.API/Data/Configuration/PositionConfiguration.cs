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
            new Position {Id = Guid.Parse("01a166be-ee55-747c-b090-69f7572f26e8"), Title = "Staff", Level = 2},
            new Position {Id = Guid.Parse("01a166be-ee55-747c-b090-6c3b9bd9cd5c"), Title = "Supervisor", Level = 5},
            new Position {Id = Guid.Parse("01a166be-ee55-747c-b090-72cbd94cb9f2"), Title = "Department Head", Level = 8}
        );
    }
}