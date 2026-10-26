using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace HRIS.API.Common.ExceptionHandlers;

/// <summary>FluentValidation failures (thrown by ValidationBehavior) become 400 with an "errors" dictionary.</summary>
public class ValidationExceptionHandler(IProblemDetailsService problemDetailsService) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken ct)
    {
        if (exception is not ValidationException validationException)
            return false;   // not mine: the next handler in the chain gets it

        // The command wraps the request, so FluentValidation reports "Request.FullName". The client reads "FullName".
        const string prefix = "Request.";
        var errors = validationException.Errors
            .GroupBy(e => e.PropertyName.StartsWith(prefix, StringComparison.Ordinal) ? e.PropertyName[prefix.Length..] : e.PropertyName)
            .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

        var problem = new ValidationProblemDetails(errors)
        {
            Title = "One or more validation errors occurred.",
            Status = StatusCodes.Status400BadRequest
        };
        problem.Extensions[ProblemDetailsWriter.ErrorCodeKey] = "VALIDATION_FAILED";

        await ProblemDetailsWriter.WriteAsync(problemDetailsService, httpContext, problem, exception, ct);
        return true;
    }
}