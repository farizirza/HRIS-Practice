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
        
        services.AddScoped<IEmployeeService, EfEmployeeService>();
        return services;
    }
}