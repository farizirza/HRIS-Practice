using HRIS.API.Common;
using HRIS.API.Data;
using HRIS.API.Entities;
using HRIS.API.Features.Employees.Commands.CreateEmployee;
using HRIS.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Time.Testing;

namespace HRIS.API.Tests;

public class CreateEmployeeCommandValidatorTests : IAsyncLifetime
{
    private readonly FakeTimeProvider _time = new(new DateTimeOffset(2026, 10, 19, 9, 0, 0, TimeSpan.Zero));
    private readonly HrisDbContext _db;
    private readonly CreateEmployeeCommandValidator _sut;
    private readonly Guid _departmentId = Guid.NewGuid();
    private readonly Guid _positionId = Guid.NewGuid();

    public CreateEmployeeCommandValidatorTests()
    {
        var options = new DbContextOptionsBuilder<HrisDbContext>()
            .UseInMemoryDatabase($"validator-{Guid.NewGuid()}")
            .Options;
        _db = new HrisDbContext(options);
        _sut = new CreateEmployeeCommandValidator(_db, _time);
    }

    public async Task InitializeAsync()
    {
        _db.Departments.Add(new Department { Id = _departmentId, Code = "ENG", Name = "Engineering" });
        _db.Positions.Add(new Position { Id = _positionId, Title = "Software Engineer" });
        await _db.SaveChangesAsync();
    }

    public async Task DisposeAsync() => await _db.DisposeAsync();

    [Fact]
    public async Task Validate_ValidCommand_IsValid()
    {
        var result = await _sut.ValidateAsync(NewCommand());

        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData("2027-01-17", true)]    // today + 90 days
    [InlineData("2027-01-18", false)]   // today + 91 days
    public async Task Validate_HireDate_AllowsAtMost90DaysAhead(string hireDate, bool expectedValid)
    {
        var result = await _sut.ValidateAsync(NewCommand(r => r with { HireDate = DateOnly.Parse(hireDate) }));

        Assert.Equal(expectedValid, result.IsValid);
    }

    [Fact]
    public async Task Validate_UnknownDepartment_FailsOnDepartmentId()
    {
        var result = await _sut.ValidateAsync(NewCommand(r => r with { DepartmentId = Guid.NewGuid() }));

        var error = Assert.Single(result.Errors);
        Assert.Equal("Request.DepartmentId", error.PropertyName);   // the command validator reports the path through Request
        Assert.Equal("Department does not exist.", error.ErrorMessage);
    }

    // Builds a valid command; pass a function to change one field of the request.
    private CreateEmployeeCommand NewCommand(Func<CreateEmployeeRequest, CreateEmployeeRequest>? change = null)
    {
        var request = new CreateEmployeeRequest(
            "EMP-0100", "Siti Aminah", "siti@company.id", new DateOnly(2026, 11, 2), _departmentId, _positionId, null);
        return new CreateEmployeeCommand(change is null ? request : change(request));
    }
}