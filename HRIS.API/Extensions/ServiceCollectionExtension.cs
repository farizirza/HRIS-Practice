using HRIS.API.Data;
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
        
        services.AddSingleton<IEmployeeService, InMemoryEmployeeService>();
        services.AddSingleton(TimeProvider.System);
        
        services.AddTransient<ITransientOperation, Operation>();   // a new instance for every injection
        services.AddScoped<IScopedOperation, Operation>();         // one instance per HTTP request
        services.AddSingleton<ISingletonOperation, Operation>();   // one instance for the application
        services.AddScoped<OperationReport>();                     // helper 
        
        return services;
    }
}