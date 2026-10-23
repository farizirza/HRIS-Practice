namespace HRIS.API.Middlewares;

public class MaintenanceModeMiddleware(IConfiguration configuration) : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (configuration.GetValue<bool>("MaintenanceMode"))
        {
            context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
            await context
                .Response
                .WriteAsync("The service is currently under maintenance. Please try again later.");
            
            return;
        }
        
        await next(context);
    }
}