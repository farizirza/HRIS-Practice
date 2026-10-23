using HRIS.API.Middlewares;

namespace HRIS.API.Extensions;

public static class MiddlewareExtensions
{
    public static IServiceCollection AddMiddleware(this IServiceCollection services)
    {
        services.AddSingleton<RequestTimingMiddleware>();
        services.AddSingleton<MaintenanceModeMiddleware>();
        return services;
    }
    
    public static IApplicationBuilder UseRequestTiming(this IApplicationBuilder builder) 
        => builder.UseMiddleware<RequestTimingMiddleware>();
    
    public static IApplicationBuilder UseMaintenanceMode(this IApplicationBuilder builder) 
        => builder.UseMiddleware<MaintenanceModeMiddleware>();
}