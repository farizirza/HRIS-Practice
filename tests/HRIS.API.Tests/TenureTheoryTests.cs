using HRIS.API.Common;
using Microsoft.Extensions.Time.Testing;

namespace HRIS.API.Tests;

public class TenureTheoryTests
{
    private readonly EmployeePolicy _policy = new(new FakeTimeProvider(new DateTimeOffset(2026, 10, 19, 9, 0, 0, TimeSpan.Zero)));

    public static TheoryData<DateOnly, int> TenureCases => new()
    {
        { new DateOnly(2026, 9, 19), 1 },
        { new DateOnly(2026, 9, 20), 0 },     // the month is not completed yet
        { new DateOnly(2023, 3, 1), 43 }
    };

    [Theory]
    [MemberData(nameof(TenureCases))]
    public void GetTenureMonths_ReturnsCompletedMonths(DateOnly hireDate, int expected)
    {
        Assert.Equal(expected, _policy.GetTenureMonths(hireDate));
    }
}