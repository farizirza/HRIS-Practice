using HRIS.API.Common.Exceptions;
using HRIS.API.Data.Repositories;
using HRIS.API.Entities;
using HRIS.API.Features.Employees.Commands.TerminateEmployee;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Time.Testing;
using Moq;

namespace HRIS.API.Tests;

public class TerminateEmployeeCommandHandlerTests
{
    private readonly FakeTimeProvider _time = new(new DateTimeOffset(2026, 10, 19, 9, 0, 0, TimeSpan.Zero));
    private readonly Mock<IEmployeeRepository> _employees = new();
    private readonly Mock<IUnitOfWork> _uow = new();
    private readonly TerminateEmployeeCommandHandler _sut;

    public TerminateEmployeeCommandHandlerTests()
    {
        _uow.Setup(u => u.Employees).Returns(_employees.Object);
        _sut = new TerminateEmployeeCommandHandler(_uow.Object, _time, NullLogger<TerminateEmployeeCommandHandler>.Instance);
    }

    [Fact]
    public async Task Handle_UnknownEmployee_ThrowsNotFoundException()
    {
        var id = Guid.NewGuid();
        _employees.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync((Employee?)null);

        var ex = await Assert.ThrowsAsync<NotFoundException>(() => _sut.Handle(new TerminateEmployeeCommand(id), CancellationToken.None));

        Assert.Equal(404, ex.StatusCode);
    }

    [Fact]
    public async Task Handle_AlreadyTerminated_ThrowsEmployeeAlreadyTerminatedException_AndSavesNothing()
    {
        var employee = NewEmployee();
        employee.Status = EmployeeStatus.Terminated;
        _employees.Setup(r => r.GetByIdAsync(employee.Id, It.IsAny<CancellationToken>())).ReturnsAsync(employee);

        await Assert.ThrowsAsync<EmployeeAlreadyTerminatedException>(() => _sut.Handle(new TerminateEmployeeCommand(employee.Id), CancellationToken.None));

        _uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ActiveEmployee_TerminatesToday_AndMovesReportsToTheManager()
    {
        var managerId = Guid.NewGuid();
        var employee = NewEmployee();
        employee.ManagerId = managerId;
        var report = NewEmployee();
        report.ManagerId = employee.Id;
        _employees.Setup(r => r.GetByIdAsync(employee.Id, It.IsAny<CancellationToken>())).ReturnsAsync(employee);
        _employees.Setup(r => r.GetDirectReportsAsync(employee.Id, It.IsAny<CancellationToken>())).ReturnsAsync([report]);

        await _sut.Handle(new TerminateEmployeeCommand(employee.Id), CancellationToken.None);

        Assert.Equal(EmployeeStatus.Terminated, employee.Status);
        Assert.Equal(new DateOnly(2026, 10, 19), employee.TerminatedAt);
        Assert.Equal(managerId, report.ManagerId);
        _uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    private static Employee NewEmployee() => new()
    {
        Id = Guid.NewGuid(), EmployeeNumber = "EMP-0001", FullName = "Test", Email = "test@company.id", Status = EmployeeStatus.Active,
        HireDate = new DateOnly(2024, 1, 1)
    };
}