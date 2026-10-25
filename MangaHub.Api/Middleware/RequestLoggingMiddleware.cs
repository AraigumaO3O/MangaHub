using System.Diagnostics;

namespace MangaHub.Api.Middleware;


public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;


    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var cronometro = Stopwatch.StartNew();

        try
        {
            await _next(context);
        }
        finally
        {
            cronometro.Stop();
            _logger.LogInformation(
                "{Metodo} {Ruta} -> {Codigo} ({Ms} ms)",
                context.Request.Method,
                context.Request.Path,
                context.Response.StatusCode,
                cronometro.ElapsedMilliseconds);
        }
    }
}
