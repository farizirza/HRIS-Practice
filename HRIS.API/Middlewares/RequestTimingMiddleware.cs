using System.Diagnostics;

namespace HRIS.API.Middlewares;

public class RequestTimingMiddleware(ILogger<RequestTimingMiddleware> logger) : IMiddleware
{
    private readonly string _correlationHeader = "X-Correlation-Id";

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        
        var correlationId = context.Request.Headers[_correlationHeader].FirstOrDefault();
        if (string.IsNullOrWhiteSpace(correlationId))
        {
            correlationId = Guid.NewGuid().ToString();
        }
        
        context.Response.Headers["X-Correlation-Id"] = correlationId;

        using (logger.BeginScope("CorrelationId: {CorrelationId}", correlationId))
        {
            await next(context);
        }

        stopwatch.Stop();
        logger.LogDebug("{Method} {Path} responded {StatusCode} in {ElapsedTime} ms", 
            context.Request.Method, 
            context.Request.Path, 
            context.Response.StatusCode, 
            stopwatch.ElapsedMilliseconds);
    }
}