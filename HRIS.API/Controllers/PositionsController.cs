using HRIS.API.Features.Positions.Commands.CreatePosition;
using HRIS.API.Features.Positions.Queries.GetPositions;
using HRIS.API.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HRIS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PositionsController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<PositionResponse>>> GetAll(CancellationToken ct)
        => Ok(await sender.Send(new GetPositionsQuery(), ct));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePositionRequest request, CancellationToken ct)
    {
        var id = await sender.Send(new CreatePositionCommand(request), ct);
        return Created($"/api/positions/{id}", new { id });
    }
}