using HRIS.API.Entities;
using HRIS.API.Models;
using HRIS.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace HRIS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeesController(IEmployeeService employeeService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<EmployeeResponse>>> GetAll(
        CancellationToken ct)
        => Ok(await employeeService.GetAllAsync(ct));
    
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<EmployeeResponse>> GetById([FromRoute]Guid id, CancellationToken ct)
    {
        var employee = await employeeService.GetByIdAsync(id, ct);
        return employee is null 
            ? NotFound() 
            : Ok(employee);
    }
    
    [HttpPost]
    public async Task<ActionResult<EmployeeResponse>> Create([FromBody]CreateEmployeeRequest request, CancellationToken ct)
    {
        var id = await employeeService.CreateAsync(request, ct);
        var employee = await employeeService.GetByIdAsync(id, ct);
        return CreatedAtAction(nameof(GetById), new { id }, employee);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateEmployeeRequest request, CancellationToken ct)
        => await employeeService.UpdateAsync(id, request, ct) 
            ? NoContent() 
            : NotFound();

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken ct)
        => await employeeService.DeleteAsync(id, ct)
            ? NoContent()
            : NotFound();

    [HttpPost("{id:guid}/terminate")]
    public async Task<IActionResult> Terminate([FromRoute] Guid id, CancellationToken ct)
        => await employeeService.TerminateAsync(id, ct)
            switch
            {
                TerminateResult.Terminated => NoContent(),
                TerminateResult.AlreadyTerminated => Conflict(),
                _ => NotFound()
            };
}