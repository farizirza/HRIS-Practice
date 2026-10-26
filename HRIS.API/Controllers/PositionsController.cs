using HRIS.API.Data;
using HRIS.API.Entities;
using HRIS.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HRIS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PositionsController(HrisDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<PositionResponse>>> GetAll(CancellationToken ct)
        => Ok(await db.Positions
            .AsNoTracking()
            .OrderByDescending(p => p.Level)
            .ThenBy(p => p.Title)
            .Select(p => p.ToResponse())
            .ToListAsync(ct));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePositionRequest request, CancellationToken ct)
    {
        var position = request.ToEntity();
        db.Positions.Add(position);
        await db.SaveChangesAsync(ct);
        
        return Created($"/api/positions/{position.Id}", new { id = position.Id });
    }
}