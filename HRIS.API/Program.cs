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
    builder.Services.AddHrisSecurity(builder.Configuration);   // Module 10, after AddHrisServices
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
    
    app.UseRequestTiming();                                     // Module 3
    app.UseHrisRequestLogging();                                // Module 5
    app.UseExceptionHandler();                                  // Module 7
    app.UseMaintenanceMode();                                   // Module 3

    // ... the Development block with MapOpenApi().AllowAnonymous() and UseSwaggerUI(...) stays here, as in Module 10 ...

    app.UseHttpsRedirection();
    app.UseCors();                                              // Module 3: before authentication
    app.UseAuthentication();                                    // builds HttpContext.User from the token. BEFORE UseAuthorization
    app.Use(async (context, next) =>                            // every log line of this request carries the user id
    {
        var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "anonymous";
        using (Serilog.Context.LogContext.PushProperty("UserId", userId))
            await next(context);
    });
    app.UseAuthorization();                                     // roles, policies, fallback policy
    app.MapControllers();

    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi().AllowAnonymous();   // the OpenAPI document is an endpoint: the fallback policy would answer 401
        app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "HRIS API"));
        await app.SeedBootstrapAdminAsync();   // Module 10: admin@hris.local (update the database first)
    }

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
