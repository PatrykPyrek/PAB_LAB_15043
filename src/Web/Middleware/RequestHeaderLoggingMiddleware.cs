using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace PPyrekBackend15043.Web.Middleware;

public class RequestHeaderLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestHeaderLoggingMiddleware> _logger;

    public RequestHeaderLoggingMiddleware(RequestDelegate next, ILogger<RequestHeaderLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {

        string requestId = Guid.NewGuid().ToString();
        _logger.LogInformation("Generated X-Request-Id: {RequestId}", requestId);


        context.Response.Headers["X-Request-Id"] = requestId;
        context.Response.Headers.Append("X-Powered-By", "Patryk-Pyrek");

        _logger.LogInformation("Incoming request: {Method} {Path}", context.Request.Method, context.Request.Path);

        foreach (var header in context.Request.Headers)
        {
            _logger.LogInformation("Header: {Key} = {Value}", header.Key, header.Value.ToString());

        }

        if (context.Request.ContentLength > 0 && context.Request.Body.CanRead)
        {
            context.Request.EnableBuffering();

            using var reader = new StreamReader(
                context.Request.Body,
                encoding: Encoding.UTF8,
                detectEncodingFromByteOrderMarks: false,
                bufferSize: 1024,
                leaveOpen: true
            );

            string body = await reader.ReadToEndAsync();
            context.Request.Body.Position = 0;

            _logger.LogInformation("Request Body:\n{Body}", body);
        }
        else
        {
            _logger.LogInformation("Request Body is empty or not readable.");
        }

        await _next(context);
    }
}

public static class RequestHeaderLoggingMiddlewareExtensions
{
    public static IApplicationBuilder UseRequestHeaderLogging(this IApplicationBuilder app)
    {
        return app.UseMiddleware<RequestHeaderLoggingMiddleware>();
    }
}
