using HRIS.API.Data.Repositories;
using HRIS.API.Entities;
using HRIS.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace HRIS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PositionsController(IUnitOfWork uow) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<PositionResponse>>> GetAll(CancellationToken ct)
        => Ok(await uow.Positions.GetResponsesAsync(ct));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePositionRequest request, CancellationToken ct)
    {
        var position = request.ToEntity();
        await uow.Positions.AddAsync(position, ct);
        await uow.SaveChangesAsync(ct);
        
        return Created($"/api/positions/{position.Id}", new { id = position.Id });
    }
}