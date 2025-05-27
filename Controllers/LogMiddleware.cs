using Microsoft.AspNetCore.Mvc;

namespace Log
{
    public class LogMiddleware
    {
        private readonly RequestDelegate _next;

        public LogMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            await context.Response.WriteAsync("This is a log.\n");
            await _next(context);
            await context.Response.WriteAsync("Log End.\n");
        }

        
    }
}