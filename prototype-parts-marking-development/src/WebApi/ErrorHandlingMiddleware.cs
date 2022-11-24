namespace WebApi
{
    using System;
    using System.Text.Json;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using Utilities;

    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ILogger<ErrorHandlingMiddleware> logger;
        private readonly IProblemDetailsFactory problemDetailsFactory;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger, IProblemDetailsFactory problemDetailsFactory)
        {
            Guard.NotNull(next, nameof(next));
            Guard.NotNull(logger, nameof(logger));
            Guard.NotNull(problemDetailsFactory, nameof(problemDetailsFactory));

            this.next = next;
            this.logger = logger;
            this.problemDetailsFactory = problemDetailsFactory;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (InvalidSortPropertyException e)
            {
                LogException(e);

                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                context.Response.ContentType = "application/json";

                var problem = problemDetailsFactory.Create(
                    400,
                    "Invalid Sort Property",
                    "Failed to sort on the provided property.");

                await context.Response.WriteAsync(JsonSerializer.Serialize(problem));
            }
            catch (ApiException e)
            {
                LogException(e);

                context.Response.StatusCode = (int)e.StatusCode;
                context.Response.ContentType = "application/json";

                await context.Response.WriteAsync(JsonSerializer.Serialize(e.ProblemDetails));
            }
            catch (ModelValidationFailedException e)
            {
                LogException(e);

                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                context.Response.ContentType = "application/json";

                await context.Response.WriteAsync(JsonSerializer.Serialize(e.ValidationProblemDetails));
            }
            catch (DbUpdateConcurrencyException e)
            {
                LogException(e);

                context.Response.StatusCode = StatusCodes.Status409Conflict;
            }
            catch (Exception e)
            {
                LogException(e);

                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            }
        }

        private void LogException<T>(T exception)
            where T : Exception
        {
            logger.LogError(exception, "Request execution failed due to {Message}", exception.Message);
        }
    }
}