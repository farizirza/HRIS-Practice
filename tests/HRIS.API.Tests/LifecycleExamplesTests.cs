using HRIS.API.Common;
using Microsoft.Extensions.Time.Testing;
using Xunit;
using Xunit.Abstractions;

namespace HRIS.API.Tests;

public class LifecycleTests : IDisposable
{
    private readonly List<string> _log = [];          // instance field: new and empty for every test

    public LifecycleTests()
    {
        _log.Add("constructor");
    }

    [Fact]
    public void FirstTest_SeesOnlyItsOwnLog()
    {
        _log.Add("first test");
        Assert.Equal(2, _log.Count);                  // "constructor" + "first test", nothing from other tests
    }

    [Fact]
    public void SecondTest_StartsWithAFreshLog()
    {
        Assert.Single(_log);                          // only "constructor": setup ran, and nothing from the first test is left
    }

    public void Dispose() => _log.Clear();            // runs after every test, pass or fail
}

public class EmployeeSeedFixture : IDisposable
{
    public IReadOnlyList<DateOnly> HireDates { get; } =
    [
        new DateOnly(2020, 1, 15), new DateOnly(2024, 6, 1), new DateOnly(2026, 9, 1)
    ];

    public void Dispose() { /* close connections, delete temp files ... */ }
}

public class FixtureTests(EmployeeSeedFixture seed, ITestOutputHelper output) : IClassFixture<EmployeeSeedFixture>
{
    private readonly EmployeePolicy _policy = new(new FakeTimeProvider(new DateTimeOffset(2026, 10, 19, 9, 0, 0, TimeSpan.Zero)));

    [Fact]
    public void ExactlyOneSeededEmployee_IsOnProbation()
    {
        var onProbation = seed.HireDates.Count(_policy.IsOnProbation);

        output.WriteLine($"Seeded {seed.HireDates.Count} hire dates, {onProbation} on probation");
        Assert.Equal(1, onProbation);
    }
}

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
