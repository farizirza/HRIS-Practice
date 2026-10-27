using FluentValidation;
using FluentValidation.Results;
using HRIS.API.Common.ExceptionHandlers;
using HRIS.API.Common.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;

namespace HRIS.API.Tests;

public class ExceptionHandlingTests
{
    private readonly CapturingProblemDetailsService _problemDetails = new();
    private readonly DefaultHttpContext _httpContext = new();

    [Fact]
    public async Task DomainExceptionHandler_ConflictException_Writes409WithErrorCode()
    {
        var sut = new DomainExceptionHandler(_problemDetails, NullLogger<DomainExceptionHandler>.Instance);
        var exception = new EmployeeAlreadyTerminatedException(Guid.NewGuid());

        var handled = await sut.TryHandleAsync(_httpContext, exception, CancellationToken.None);

        Assert.True(handled);
        Assert.Equal(StatusCodes.Status409Conflict, _httpContext.Response.StatusCode);
        var problem = _problemDetails.Written!.ProblemDetails;
        Assert.Equal(exception.Message, problem.Detail);
        Assert.Equal("EMPLOYEE_ALREADY_TERMINATED", problem.Extensions[ProblemDetailsWriter.ErrorCodeKey]);
    }

    [Fact]
    public async Task ValidationExceptionHandler_GroupsErrorsPerProperty_AndRemovesRequestPrefix()
    {
        var sut = new ValidationExceptionHandler(_problemDetails);
        var exception = new ValidationException(
        [
            new ValidationFailure("Request.Email", "'Email' must not be empty."),
            new ValidationFailure("Request.Email", "'Email' is not a valid email address."),
            new ValidationFailure("Request.FullName", "'Full Name' must not be empty.")
        ]);

        var handled = await sut.TryHandleAsync(_httpContext, exception, CancellationToken.None);

        Assert.True(handled);
        Assert.Equal(StatusCodes.Status400BadRequest, _httpContext.Response.StatusCode);
        var problem = Assert.IsType<ValidationProblemDetails>(_problemDetails.Written!.ProblemDetails);
        Assert.Equal(2, problem.Errors["Email"].Length);
        Assert.Single(problem.Errors["FullName"]);
        Assert.Equal("VALIDATION_FAILED", problem.Extensions[ProblemDetailsWriter.ErrorCodeKey]);
    }

    [Fact]
    public async Task GlobalExceptionHandler_UnexpectedException_Returns500WithoutInternalDetails()
    {
        var sut = new GlobalExceptionHandler(NullLogger<GlobalExceptionHandler>.Instance, _problemDetails);
        var exception = new InvalidOperationException("Connection string 'Server=prod-sql-01;Password=secret' failed");

        var handled = await sut.TryHandleAsync(_httpContext, exception, CancellationToken.None);

        Assert.True(handled);
        Assert.Equal(StatusCodes.Status500InternalServerError, _httpContext.Response.StatusCode);
        var problem = _problemDetails.Written!.ProblemDetails;
        Assert.DoesNotContain("secret", problem.Detail);
        Assert.DoesNotContain("prod-sql-01", problem.Detail);
        Assert.Equal("INTERNAL_ERROR", problem.Extensions[ProblemDetailsWriter.ErrorCodeKey]);
    }

    /// <summary>Captures what a handler writes instead of serializing it to the response body.</summary>
    private sealed class CapturingProblemDetailsService : IProblemDetailsService
    {
        public ProblemDetailsContext? Written { get; private set; }

        public ValueTask WriteAsync(ProblemDetailsContext context)
        {
            Written = context;
            return ValueTask.CompletedTask;
        }

        public ValueTask<bool> TryWriteAsync(ProblemDetailsContext context)
        {
            Written = context;
            return ValueTask.FromResult(true);
        }
    }
}