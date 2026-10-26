using FluentValidation;
using FluentValidation.Results;
using MediatR;

namespace HRIS.API.Common.Behaviors;

/// <summary>
/// Runs every FluentValidation validator for the request and throws ONE ValidationException with all failures.
/// The validators run one after another, not with Task.WhenAll, because they share the scoped DbContext.
/// </summary>
public class ValidationBehavior<TRequest, TResponse>(
    IEnumerable<IValidator<TRequest>> validators,
    ILogger<ValidationBehavior<TRequest, TResponse>> logger) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(
        TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var failures = new List<ValidationFailure>();
        foreach (var validator in validators)
        {
            var result = await validator.ValidateAsync(new ValidationContext<TRequest>(request), cancellationToken);
            failures.AddRange(result.Errors);
        }

        if (failures.Count != 0)
        {
            // Property NAMES only: the attempted values may be personal data or a password.
            logger.LogInformation("Validation failed for {RequestName}: {ErrorCount} error(s) on {Properties}",
                typeof(TRequest).Name, failures.Count, failures.Select(f => f.PropertyName).Distinct().ToArray());
            throw new ValidationException(failures);
        }

        return await next();   // valid: run the next behavior or the handler
    }
}