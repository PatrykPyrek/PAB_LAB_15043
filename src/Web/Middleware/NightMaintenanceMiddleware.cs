

namespace PPyrekBackend15043.Web.Middleware;

public class NightBlockMiddleware
{
    private readonly RequestDelegate _next;

    public NightBlockMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var hour = DateTime.Now.Hour;

        if (hour >= 0 && hour < 6)
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsync("API unavaliable: 00:00 - 06:00.");
            return;
        }

        await _next(context);
    }
}

public static class NightBlockMiddlewareExtensions
{
    public static IApplicationBuilder UseNightBlockMiddleware(this IApplicationBuilder app)
    {
        return app.UseMiddleware<NightBlockMiddleware>();
    }
}
