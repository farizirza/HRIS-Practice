using HRIS.API.Common.Behaviors;
using HRIS.API.Common.ExceptionHandlers;
using HRIS.API.Data;
using HRIS.API.Data.Repositories;
using HRIS.API.Services;
using Microsoft.EntityFrameworkCore;

namespace HRIS.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddHrisServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("HrisDB");
        services.AddDbContext<HrisDbContext>(options => 
            options.UseSqlServer(connectionString));
        
        services.AddSingleton(TimeProvider.System);
        
        services.AddTransient<ITransientOperation, Operation>();   // a new instance for every injection
        services.AddScoped<IScopedOperation, Operation>();         // one instance per HTTP request
        services.AddSingleton<ISingletonOperation, Operation>();   // one instance for the application
        services.AddScoped<OperationReport>();                     // helper 
        
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>)); // IRepository<Employee>.. IRepository<Position>..
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IEmployeeRepository, EmployeeRepository>();
        services.AddScoped<IDepartmentRepository, DepartmentRepository>();
        services.AddScoped<IPositionRepository, PositionRepository>();
        
        // Module 6: MediatR scans this assembly for handlers
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblyContaining<Program>();
            cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
        });
        
        services.AddProblemDetails(options => options.CustomizeProblemDetails = context =>
        {
            var problem = context.ProblemDetails;
            problem.Instance ??= context.HttpContext.Request.Path.Value;

            // The exception handler has already cleared the response headers, so read the id from Items (Step 5).
            if (context.HttpContext.Items.TryGetValue("X-Correlation-Id", out var correlationId))
            {
                problem.Extensions["correlationId"] = correlationId?.ToString();
            }
        });
        services.AddExceptionHandler<GlobalExceptionHandler>();   // Lesson 3 adds two more BEFORE this line
        return services;
    }
}