namespace WebApi
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Utilities;

    public class CorrelationIdMiddleware
    {
        private const string Header = "X-Correlation-ID";

        private readonly RequestDelegate next;

        public CorrelationIdMiddleware(RequestDelegate next)
        {
            Guard.NotNull(next, nameof(next));

            this.next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Headers.TryGetValue(Header, out var result))
            {
                context.TraceIdentifier = result;
            }

            context.Response.OnStarting(() =>
            {
                context.Response.Headers.Add(Header, context.TraceIdentifier);
                return Task.CompletedTask;
            });

            await next(context);
        }
    }
}