using HRIS.API.Common.Exceptions;
using HRIS.API.Data.Repositories;
using HRIS.API.Entities;
using HRIS.API.Features.Departments.Commands.CreateDepartment;
using HRIS.API.Models;
using Moq;

namespace HRIS.API.Tests;

public class CreateDepartmentCommandHandlerTests
{
    private readonly Mock<IDepartmentRepository> _departments = new();
    private readonly Mock<IUnitOfWork> _uow = new();
    private readonly CreateDepartmentCommandHandler _sut;

    public CreateDepartmentCommandHandlerTests()
    {
        _uow.Setup(u => u.Departments).Returns(_departments.Object);
        _sut = new CreateDepartmentCommandHandler(_uow.Object);
    }

    [Fact]
    public async Task Handle_DuplicateCode_ThrowsDuplicateDepartmentException_AndSavesNothing()
    {
        _departments.Setup(r => r.CodeExistsAsync("TECH", It.IsAny<CancellationToken>())).ReturnsAsync(true);
        var command = new CreateDepartmentCommand(new CreateDepartmentRequest(" tech ", "Technology Copy", null));

        var ex = await Assert.ThrowsAsync<DuplicateDepartmentException>(() => _sut.Handle(command, CancellationToken.None));

        Assert.Equal(409, ex.StatusCode);
        Assert.Contains("TECH", ex.Message);
        _departments.Verify(r => r.AddAsync(It.IsAny<Department>(), It.IsAny<CancellationToken>()), Times.Never);
        _uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_NewCode_StoresTheCodeTrimmedAndUpperCased()
    {
        var command = new CreateDepartmentCommand(new CreateDepartmentRequest(" qa ", "Quality Assurance", null));

        var id = await _sut.Handle(command, CancellationToken.None);

        _departments.Verify(r => r.AddAsync(
            It.Is<Department>(d => d.Id == id && d.Code == "QA" && d.Name == "Quality Assurance"),
            It.IsAny<CancellationToken>()), Times.Once);
        _uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}