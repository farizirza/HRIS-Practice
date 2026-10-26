using System.Security.Claims;
using Serilog;
using Serilog.Events;

namespace HRIS.API.Extensions;

public static class LoggingExtensions
{
    /// <summary>Replaces the default logging providers with Serilog, configured from the "Serilog" section of appsettings.</summary>
    public static WebApplicationBuilder AddHrisSerilog(this WebApplicationBuilder builder)
    {
        if (builder.Environment.IsDevelopment())
        {
            Serilog.Debugging.SelfLog.Enable(Console.Error);   // prints why a sink or a configuration entry does not work
        }

        builder.Services.AddSerilog((services, configuration) => configuration
            .ReadFrom.Configuration(builder.Configuration)          // levels, overrides, sinks, the "Application" property
            .ReadFrom.Services(services)
            .Enrich.FromLogContext()                                // properties pushed with LogContext.PushProperty
            .Enrich.WithProperty("Environment", builder.Environment.EnvironmentName));

        return builder;
    }

    /// <summary>
    /// One structured line per request, written when the response is finished, so it knows the final status code.
    /// Register it near the top of the pipeline, and in Module 7 BEFORE UseExceptionHandler.
    /// </summary>
    public static IApplicationBuilder UseHrisRequestLogging(this IApplicationBuilder app)
        => app.UseSerilogRequestLogging(options =>
        {
            options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0} ms";
            options.GetLevel = GetLevel;
            options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
            {
                // Runs at the END of the request.
                diagnosticContext.Set("UserId", httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "anonymous");
                diagnosticContext.Set("CorrelationId", httpContext.Response.Headers["X-Correlation-Id"].ToString());
                // Not logged on purpose: bodies, query string, Authorization header, client IP (personal data and secrets).
            };
        });

    /// <summary>5xx = Error (our bug), 4xx = Warning (client problem), everything else = Information.</summary>
    private static LogEventLevel GetLevel(HttpContext httpContext, double elapsedMs, Exception? exception)
        => exception is not null || httpContext.Response.StatusCode >= StatusCodes.Status500InternalServerError
            ? LogEventLevel.Error
            : httpContext.Response.StatusCode >= StatusCodes.Status400BadRequest
                ? LogEventLevel.Warning
                : LogEventLevel.Information;
}