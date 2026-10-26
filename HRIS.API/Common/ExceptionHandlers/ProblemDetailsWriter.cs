using Microsoft.AspNetCore.Mvc;

namespace HRIS.API.Common.ExceptionHandlers;

public static class ProblemDetailsWriter
{
    public const string ErrorCodeKey = "errorCode";

    /// <summary>
    /// Writes through IProblemDetailsService, so every error response gets "type", "traceId" and whatever
    /// CustomizeProblemDetails adds (instance, correlationId). Falls back to plain JSON when the client
    /// sent an Accept header that excludes JSON.
    /// </summary>
    public static async ValueTask WriteAsync(
        IProblemDetailsService service, HttpContext httpContext, ProblemDetails problem, Exception exception, CancellationToken ct)
    {
        httpContext.Response.StatusCode = problem.Status ?? StatusCodes.Status500InternalServerError;

        var written = await service.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = problem,
            Exception = exception
        });

        if (!written)
            await httpContext.Response.WriteAsJsonAsync(problem, ct);
    }
}