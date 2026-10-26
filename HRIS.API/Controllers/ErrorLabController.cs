using Microsoft.AspNetCore.Mvc;

namespace HRIS.API.Controllers;

[ApiController]
[Route("api/error-lab")]
public class ErrorLabController : ControllerBase
{
    // An unexpected failure with a secret in the message: the response must not contain it.
    [HttpGet("boom")]
    public IActionResult Boom()
        => throw new InvalidOperationException("Connection string 'Server=prod-sql-01;Password=s3cret' failed");

    // throw; versus throw ex; : compare the stack trace in the log (the response is a 500 either way).
    [HttpGet("rethrow")]
    public IActionResult Rethrow(bool resetStack)
    {
        try
        {
            ReadEmployeeFile();
        }
        catch (InvalidOperationException ex)
        {
            if (resetStack)
            {
                throw ex;   // WRONG: the stack trace restarts here, ReadEmployeeFile disappears from it
            }

            throw;          // RIGHT: the original stack trace is kept
        }

        return Ok();
    }

    private static void ReadEmployeeFile() => throw new InvalidOperationException("Employee file is locked");
}