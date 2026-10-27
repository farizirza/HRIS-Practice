using HRIS.API.Common.Exceptions;
using HRIS.API.Data.Repositories;
using HRIS.API.Entities;
using HRIS.API.Features.Employees.Commands.CreateEmployee;
using HRIS.API.Models;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Time.Testing;
using Moq;

namespace HRIS.API.Tests;

public class CreateEmployeeCommandHandlerTests
{
    private readonly FakeTimeProvider _time = new(new DateTimeOffset(2026, 10, 19, 9, 0, 0, TimeSpan.Zero));
    private readonly Mock<IEmployeeRepository> _employees = new();
    private readonly Mock<IUnitOfWork> _uow = new();
    private readonly CreateEmployeeCommandHandler _sut;

    private static readonly CreateEmployeeCommand Command = new(new CreateEmployeeRequest(
        "EMP-0100", "Siti Aminah", " Siti.Aminah@Company.ID ", new DateOnly(2026, 11, 2),
        Guid.NewGuid(), Guid.NewGuid(), null));

    public CreateEmployeeCommandHandlerTests()
    {
        _uow.Setup(u => u.Employees).Returns(_employees.Object);
        _sut = new CreateEmployeeCommandHandler(_uow.Object, _time, NullLogger<CreateEmployeeCommandHandler>.Instance);
    }

    [Fact]
    public async Task Handle_NewEmployee_AddsEmployeeAndSavesOnce()
    {
        var id = await _sut.Handle(Command, CancellationToken.None);

        _employees.Verify(r => r.AddAsync(
            It.Is<Employee>(e => e.Id == id && e.Email == "siti.aminah@company.id" && e.EmployeeNumber == "EMP-0100"
                                 && e.Status == EmployeeStatus.Active   // the handler sets the default status
                                 && e.CreatedAt == _time.GetUtcNow()),   // CreatedAt comes from the fake clock: 2026-10-19 09:00 UTC
            It.IsAny<CancellationToken>()), Times.Once);
        _uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_DuplicateEmployeeNumber_ThrowsDuplicateEmployeeException_AndSavesNothing()
    {
        _employees.Setup(r => r.EmployeeNumberExistsAsync("EMP-0100", It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var ex = await Assert.ThrowsAsync<DuplicateEmployeeException>(() => _sut.Handle(Command, CancellationToken.None));

        Assert.Contains("EMP-0100", ex.Message);
        _employees.Verify(r => r.AddAsync(It.IsAny<Employee>(), It.IsAny<CancellationToken>()), Times.Never);
        _uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}