using HRIS.API.Entities;
using HRIS.API.Models;
using HRIS.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace HRIS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeesController(IEmployeeService employeeService) : ControllerBase
{
    private readonly IEmployeeService? _employeeService = employeeService;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<EmployeeResponse>>> GetAll(
        [FromQuery]string? name,
        CancellationToken ct)
    {
        var employees = await _employeeService.GetAllAsync(ct);
        if (!string.IsNullOrWhiteSpace(name))
        {
            employees = employees
                .Where(e => e.FullName.Contains(name, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }
        
        return Ok(employees.Select(e => e.ToResponse()));
    }
    
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<EmployeeResponse>> GetById(
        [FromRoute]Guid id, 
        CancellationToken ct)
    {
        var employee = await _employeeService.GetByIdAsync(id, ct);
        if (employee is null)
        {
            return NotFound();
        }
        
        return Ok(employee.ToResponse());
    }
    
    [HttpPost]
    public async Task<ActionResult<EmployeeResponse>> Create(
        [FromBody]CreateEmployeeRequest request, 
        CancellationToken ct)
    {
        var employee = new Employee
        {
            FullName = request.FullName,
            Email = request.Email,
            EmployeeNumber = request.EmployeeNumber,
            HireDate = request.HireDate
        };
        
        await _employeeService.CreateAsync(employee, ct);
        return CreatedAtAction(nameof(GetById), new { id = employee.Id }, employee);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        [FromRoute]Guid id, 
        [FromBody]UpdateEmployeeRequest request,
        CancellationToken ct)
    {
        var employee = new Employee
        {
            EmployeeNumber = request.EmployeeNumber,
            FullName = request.FullName,
            Email = request.Email,
            HireDate = request.HireDate
        };
        
        var updated = await _employeeService.UpdateAsync(id, employee, ct);
        return updated ? NoContent() : NotFound();      
    }
    
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete([FromRoute]Guid id, CancellationToken ct)
    {
        var deleted = await _employeeService.DeleteAsync(id, ct);
        return deleted ? NoContent() : NotFound();
    }
}