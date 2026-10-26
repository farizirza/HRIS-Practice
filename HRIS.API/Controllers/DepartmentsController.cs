using HRIS.API.Data;
using HRIS.API.Entities;
using HRIS.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HRIS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DepartmentsController(HrisDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<DepartmentResponse>>> GetAll(CancellationToken ct)
        => Ok(await db.Departments
            .AsNoTracking()                                   
            .OrderBy(d => d.Code)
            .Select(d => d.ToResponse(
                d.ParentDepartment != null
                    ? d.ParentDepartment.Name
                    : null))
            .ToListAsync(ct));

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
        
        db.Departments.Add(department);
        await db.SaveChangesAsync(ct);
        return Created($"/api/departments/{department.Id}", new { id = department.Id });
    }
}