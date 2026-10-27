// tests/HRIS.API.Tests/CollectionFixtureExamplesTests.cs
using HRIS.API.Data;
using HRIS.API.Entities;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace HRIS.API.Tests;

public class SeededHrisDatabaseFixture : IDisposable
{
    public DbContextOptions<HrisDbContext> Options { get; }
    public Guid EngineeringId { get; } = Guid.NewGuid();

    public SeededHrisDatabaseFixture()
    {
        Options = new DbContextOptionsBuilder<HrisDbContext>().UseInMemoryDatabase("shared-seed").Options;

        using var db = new HrisDbContext(Options);
        db.Departments.Add(new Department { Id = EngineeringId, Code = "ENG", Name = "Engineering" });
        db.Departments.Add(new Department { Id = Guid.NewGuid(), Code = "HR", Name = "Human Resources" });
        db.SaveChanges();
    }

    public void Dispose() { /* nothing to release for the in-memory provider */ }
}

[CollectionDefinition("Seeded HRIS database")]
public class SeededHrisDatabaseCollection : ICollectionFixture<SeededHrisDatabaseFixture> { }

[Collection("Seeded HRIS database")]
public class DepartmentCountTests(SeededHrisDatabaseFixture seed)
{
    [Fact]
    public async Task Departments_AreSeededOnce()
    {
        await using var db = new HrisDbContext(seed.Options);   // a new context per test, the same database

        Assert.Equal(2, await db.Departments.CountAsync());
    }
}

[Collection("Seeded HRIS database")]
public class DepartmentLookupTests(SeededHrisDatabaseFixture seed)
{
    [Fact]
    public async Task Department_ByCode_ReturnsEngineering()
    {
        await using var db = new HrisDbContext(seed.Options);

        var department = await db.Departments.SingleAsync(d => d.Code == "ENG");

        Assert.Equal(seed.EngineeringId, department.Id);
    }
}