using HRIS.API.Controllers;
using HRIS.API.Entities;
using HRIS.API.Features.Employees.Commands.CreateEmployee;
using HRIS.API.Features.Employees.Queries.GetEmployeeById;
using HRIS.API.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace HRIS.API.Tests;

public class EmployeesControllerTests
{
    private readonly Mock<ISender> _sender = new();
    private readonly EmployeesController _sut;

    public EmployeesControllerTests()
    {
        _sut = new EmployeesController(_sender.Object);
    }

    [Fact]
    public async Task GetById_ExistingEmployee_ReturnsOkWithResponse()
    {
        var id = Guid.NewGuid();
        var response = new EmployeeResponse(id, "EMP-0003", "Siti Aminah", "siti@company.id", new DateOnly(2026, 9, 1),
            EmployeeStatus.Active, "Backend", "Software Engineer", null,
            new DateTimeOffset(2026, 9, 1, 8, 0, 0, TimeSpan.Zero));   // CreatedAt: the last parameter of EmployeeResponse
        _sender.Setup(s => s.Send(It.Is<GetEmployeeByIdQuery>(q => q.Id == id), It.IsAny<CancellationToken>()))
               .ReturnsAsync(response);

        var result = await _sut.GetById(id, CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Same(response, ok.Value);
    }

    [Fact]
    public async Task Create_ValidRequest_ReturnsCreatedAtGetByIdWithEmployeeResponse()
    {
        var newId = Guid.NewGuid();
        var request = new CreateEmployeeRequest("EMP-0042", "Siti Aminah", "siti@company.id", new DateOnly(2026, 11, 2),
            Guid.NewGuid(), Guid.NewGuid(), null);
        var response = new EmployeeResponse(newId, "EMP-0042", "Siti Aminah", "siti@company.id", new DateOnly(2026, 11, 2),
            EmployeeStatus.Active, "Backend", "Software Engineer", null,
            new DateTimeOffset(2026, 10, 19, 9, 0, 0, TimeSpan.Zero));
        // Create sends two requests: the command (returns the new id), then the query (returns the full response).
        _sender.Setup(s => s.Send(It.IsAny<CreateEmployeeCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(newId);
        _sender.Setup(s => s.Send(It.Is<GetEmployeeByIdQuery>(q => q.Id == newId), It.IsAny<CancellationToken>())).ReturnsAsync(response);

        var result = await _sut.Create(request, CancellationToken.None);

        var created = Assert.IsType<CreatedAtActionResult>(result.Result);   // Create returns ActionResult<EmployeeResponse>
        Assert.Equal(nameof(EmployeesController.GetById), created.ActionName);
        Assert.Equal(newId, created.RouteValues!["id"]);
        Assert.Same(response, created.Value);
    }
}