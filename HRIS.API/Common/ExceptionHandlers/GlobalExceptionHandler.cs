using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace HRIS.API.Common.ExceptionHandlers;

/// <summary>
/// Catch-all handler: it always returns true, so register it last.
/// It does not put exception.Message or the stack trace in the response (SQL, file paths, server names and
/// personal data could leak). Details go to the log; the client gets the traceId to quote to support.
/// </summary>
public class GlobalExceptionHandler(
    ILogger<GlobalExceptionHandler> logger,
    IProblemDetailsService problemDetailsService) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken ct)
    {
        // The exception goes in as the FIRST argument: Serilog stores the type, message, stack trace and inner exceptions.
        logger.LogError(exception, "Unhandled {ExceptionType} while processing {Method} {Path}",
            exception.GetType().Name, httpContext.Request.Method, httpContext.Request.Path);

        var problem = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "An unexpected error occurred.",
            Detail = "Please contact support and quote the traceId."
        };
        problem.Extensions[ProblemDetailsWriter.ErrorCodeKey] = "INTERNAL_ERROR";

        await ProblemDetailsWriter.WriteAsync(problemDetailsService, httpContext, problem, exception, ct);
        return true;
    }
}