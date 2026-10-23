using HRIS.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace HRIS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DiagnosticsController(
    ITransientOperation transient,
    IScopedOperation scoped,
    ISingletonOperation singleton,
    OperationReport report) : ControllerBase
{
    // GET /api/diagnostics/lifetimes
    [HttpGet("lifetimes")]
    public IActionResult GetLifetimes() => Ok(new
    {
        Transient = new { Controller = transient.OperationId, Helper = report.Transient.OperationId },
        Scoped = new { Controller = scoped.OperationId, Helper = report.Scoped.OperationId },
        Singleton = new { Controller = singleton.OperationId, Helper = report.Singleton.OperationId }
    });
}