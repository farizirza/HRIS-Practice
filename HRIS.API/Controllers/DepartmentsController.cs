using HRIS.API.Data.Repositories;
using HRIS.API.Entities;
using HRIS.API.Features.Departments.Queries.GetDepartments;
using HRIS.API.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HRIS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DepartmentsController(IUnitOfWork uow, ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<DepartmentResponse>>> GetAll(CancellationToken ct)
        => Ok(await sender.Send(new GetDepartmentsQuery(), ct));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateDepartmentRequest request, CancellationToken ct)
    {
        var department = new Department
        {
            Id = Guid.CreateVersion7(),
            Code = request.Code.Trim().ToUpperInvariant(),
            Name = request.Name.Trim(),
            ParentDepartmentId = request.ParentDepartmentId
        };
        
        await uow.Departments.AddAsync(department, ct);
        await uow.SaveChangesAsync(ct);
        return Created($"/api/departments/{department.Id}", new { id = department.Id });
    }
}