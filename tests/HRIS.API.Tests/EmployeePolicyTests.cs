using HRIS.API.Common;
using HRIS.API.Common.Exceptions;
using HRIS.API.Entities;
using Microsoft.Extensions.Time.Testing;
using Xunit;

namespace HRIS.API.Tests;

public class EmployeePolicyTests
{
    private readonly FakeTimeProvider _time = new(new DateTimeOffset(2026, 10, 19, 9, 0, 0, TimeSpan.Zero));
    private readonly EmployeePolicy _sut;

    public EmployeePolicyTests()
    {
        _sut = new EmployeePolicy(_time);   // runs before EACH test: a fresh policy and clock
    }

    [Theory]
    [InlineData("2026-09-01", true)]    // 1.5 months in
    [InlineData("2026-07-19", false)]   // exactly 3 months: probation ends today
    [InlineData("2026-07-20", true)]    // one day short of 3 months
    public void IsOnProbation_ReturnsExpected(string hireDate, bool expected)
    {
        Assert.Equal(expected, _sut.IsOnProbation(DateOnly.Parse(hireDate)));
    }

    [Fact]
    public void GetTenureMonths_AfterTimeAdvances_Increases()
    {
        var hireDate = new DateOnly(2026, 9, 19);
        Assert.Equal(1, _sut.GetTenureMonths(hireDate));

        _time.Advance(TimeSpan.FromDays(31));   // 2026-11-19

        Assert.Equal(2, _sut.GetTenureMonths(hireDate));
    }

    [Fact]
    public void EnsureCanManage_ValidManager_DoesNotThrow()
    {
        var exception = Record.Exception(() =>
            _sut.EnsureCanManage(NewEmployee("2023-03-01"), 6, NewEmployee("2026-09-01"), 3));

        Assert.Null(exception);
    }

    [Fact]
    public void EnsureCanManage_SamePerson_Throws()
    {
        var employee = NewEmployee("2023-03-01");

        var ex = Assert.Throws<InvalidReportingLineException>(() => _sut.EnsureCanManage(employee, 6, employee, 3));

        Assert.Contains("own manager", ex.Message);
    }

    [Theory]
    [InlineData(4, 4)]
    [InlineData(3, 5)]
    public void EnsureCanManage_ManagerLevelNotHigher_Throws(int managerLevel, int employeeLevel)
    {
        Assert.Throws<InvalidReportingLineException>(() =>
            _sut.EnsureCanManage(NewEmployee("2020-01-01"), managerLevel, NewEmployee("2024-01-01"), employeeLevel));
    }

    private static Employee NewEmployee(string hireDate) => new()
    {
        Id = Guid.NewGuid(),
        EmployeeNumber = "EMP-0000",
        FullName = "Test",
        Email = "test@company.id",
        Status = EmployeeStatus.Active,
        HireDate = DateOnly.Parse(hireDate)
    };
}
