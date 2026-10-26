using HRIS.API.Features.Departments.Commands.CreateDepartment;
using HRIS.API.Features.Departments.Queries.GetDepartments;
using HRIS.API.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HRIS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DepartmentsController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<DepartmentResponse>>> GetAll(CancellationToken ct)
        => Ok(await sender.Send(new GetDepartmentsQuery(), ct));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateDepartmentRequest request, CancellationToken ct)
    {
        var id = await sender.Send(new CreateDepartmentCommand(request), ct);
        return Created($"/api/departments/{id}", new { id });
    }
}