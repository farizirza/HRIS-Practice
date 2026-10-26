using System.Security.Claims;
using System.Text.Json.Serialization;
using HRIS.API.Extensions;
using Serilog;

// Bootstrap logger: catches errors that happen BEFORE the host is built (a broken appsettings.json, for example).
// AddHrisSerilog replaces it with the logger configured from appsettings.
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("Starting HRIS API");
    var builder = WebApplication.CreateBuilder(args);

    builder.AddHrisSerilog();
    builder.Services.AddControllers(options =>
        {
            // Without this, MVC adds an implicit [Required] to every non-nullable string and answers with its own 400 format
            // BEFORE FluentValidation runs. With it, FluentValidation is the only place that rejects missing values.
            options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
        })
        .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
    builder.Services.AddHrisServices(builder.Configuration);
    builder.Services.AddMiddleware();

    builder.Services.AddCors(options =>
    {
        options.AddDefaultPolicy(policy =>
        {
            policy.AllowAnyHeader();
            policy.AllowAnyMethod();
            policy.AllowAnyOrigin();   // class demo only; production lists the allowed origins
        });
    });

    builder.Services.AddOpenApi();                    // built-in OpenAPI document, from Module 1

    var app = builder.Build();
    
    app.UseRequestTiming();
    app.UseHrisRequestLogging();     // one structured line per request, OUTSIDE the exception handler: it sees the final status
    app.UseExceptionHandler();       // turns every exception below it into a ProblemDetails response
    app.UseMaintenanceMode();

    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();                             // /openapi/v1.json
        app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "HRIS API"));
    }

    app.UseHttpsRedirection();
    app.UseCors();

    app.Use(async (context, next) =>                  // every log line written while this request runs carries a UserId
    {
        var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "anonymous";
        using (Serilog.Context.LogContext.PushProperty("UserId", userId))
        {
            await next(context);
        }
    });

    app.UseAuthorization();
    app.MapControllers();

    await app.RunAsync();
}
catch (Exception ex) when (ex is not HostAbortedException)   // 'dotnet ef' stops the host on purpose: not a failure
{
    Log.Fatal(ex, "HRIS API terminated unexpectedly");
}
finally
{
    Log.Information("HRIS API stopped");
    Log.CloseAndFlush();                              // write the last buffered events before the process exits
}