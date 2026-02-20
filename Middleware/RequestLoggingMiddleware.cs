using System.Diagnostics;
using WarSim.Logging;

namespace WarSim.Middleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var sw = Stopwatch.StartNew();
            var request = context.Request;
            ConsoleColorLogger.Log("API", Microsoft.Extensions.Logging.LogLevel.Information, $"Incoming {request.Method} {request.Path}");
            await _next(context);
            sw.Stop();
            var status = context.Response?.StatusCode;
            ConsoleColorLogger.Log("API", Microsoft.Extensions.Logging.LogLevel.Information, $"Handled {request.Method} {request.Path} => {status} in {sw.ElapsedMilliseconds}ms");
        }
    }
}
