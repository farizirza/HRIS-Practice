using HRIS.API.Common.Paging;
using HRIS.API.Entities;
using HRIS.API.Features.Employees.Commands.AssignManager;
using HRIS.API.Features.Employees.Commands.CreateEmployee;
using HRIS.API.Features.Employees.Commands.DeleteEmployee;
using HRIS.API.Features.Employees.Commands.TerminateEmployee;
using HRIS.API.Features.Employees.Commands.UpdateEmployee;
using HRIS.API.Features.Employees.Queries.GetEmployeeById;
using HRIS.API.Features.Employees.Queries.GetEmployees;
using HRIS.API.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HRIS.API.Controllers;

/// <summary>Thin controller: translate HTTP to a command or query, send it, translate the result back to HTTP.</summary>
[ApiController]
[Route("api/[controller]")]   // EmployeesController -> /api/employees
public class EmployeesController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResult<EmployeeListDto>>> GetAll(
        [FromQuery] string? search, [FromQuery] Guid? departmentId, [FromQuery] EmployeeStatus? status,
        [FromQuery] string? sortBy, [FromQuery] bool desc = false,
        [FromQuery] int page = 1, [FromQuery] int pageSize = 10,
        CancellationToken ct = default)
        => Ok(await sender.Send(new GetEmployeesQuery(search, departmentId, status, sortBy, desc, page, pageSize), ct));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<EmployeeResponse>> GetById([FromRoute] Guid id, CancellationToken ct)
        => await sender.Send(new GetEmployeeByIdQuery(id), ct) is { } employee ? Ok(employee) : NotFound();

    [HttpPost]
    [ProducesResponseType<EmployeeResponse>(StatusCodes.Status201Created)]
    public async Task<ActionResult<EmployeeResponse>> Create([FromBody] CreateEmployeeRequest request, CancellationToken ct)
    {
        var id = await sender.Send(new CreateEmployeeCommand(request), ct);
        var employee = await sender.Send(new GetEmployeeByIdQuery(id), ct);
        return CreatedAtAction(nameof(GetById), new { id }, employee);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateEmployeeRequest request, CancellationToken ct)
        => await sender.Send(new UpdateEmployeeCommand(id, request), ct) ? NoContent() : NotFound();

    [HttpPut("{id:guid}/manager")]
    public async Task<IActionResult> AssignManager([FromRoute] Guid id, [FromBody] AssignManagerRequest request, CancellationToken ct)
        => await sender.Send(new AssignManagerCommand(id, request.ManagerId), ct) ? NoContent() : NotFound();

    [HttpPost("{id:guid}/terminate")]
    public async Task<IActionResult> Terminate([FromRoute] Guid id, CancellationToken ct)
        => await sender.Send(new TerminateEmployeeCommand(id), ct) switch
        {
            TerminateResult.Terminated => NoContent(),
            TerminateResult.AlreadyTerminated => Conflict(),
            _ => NotFound()
        };

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken ct)
        => await sender.Send(new DeleteEmployeeCommand(id), ct) ? NoContent() : NotFound();
}