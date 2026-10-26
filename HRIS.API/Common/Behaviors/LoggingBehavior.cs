using System.Diagnostics;
using MediatR;

namespace HRIS.API.Common.Behaviors;

/// <summary>
/// Wraps every command and query: adds the request name to the log lines written while the handler runs
/// and warns about slow requests. It logs the request name, not the request object, because commands contain personal data.
/// </summary>
public class LoggingBehavior<TRequest, TResponse>(ILoggerFactory loggerFactory) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public const int SlowRequestThresholdMs = 500;

    // A fixed category name. ILogger<LoggingBehavior<TRequest, TResponse>> would also work: its category is the type name without the generic arguments.
    private readonly ILogger _logger = loggerFactory.CreateLogger("HRIS.API.Pipeline");

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;

        using (_logger.BeginScope(new Dictionary<string, object> { ["RequestName"] = requestName }))
        {
            _logger.LogDebug("Handling {RequestName}", requestName);
            var startedAt = Stopwatch.GetTimestamp();

            var response = await next();

            var elapsedMs = Stopwatch.GetElapsedTime(startedAt).TotalMilliseconds;
            if (elapsedMs > SlowRequestThresholdMs)
            {
                _logger.LogWarning("Slow request {RequestName} took {ElapsedMs:0} ms (threshold {ThresholdMs} ms)",
                    requestName, elapsedMs, SlowRequestThresholdMs);
            }
            else
            {
                _logger.LogDebug("Handled {RequestName} in {ElapsedMs:0} ms", requestName, elapsedMs);
            }

            return response;
        }
    }
}