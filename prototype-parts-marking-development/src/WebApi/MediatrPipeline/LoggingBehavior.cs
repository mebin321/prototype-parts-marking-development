namespace WebApi.MediatrPipeline
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using Microsoft.Extensions.Logging;
    using Utilities;

    public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly ILogger<LoggingBehavior<TRequest, TResponse>> logger;

        public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
        {
            Guard.NotNull(logger, nameof(logger));

            this.logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            logger.LogTrace("Executing {@Request}", request);

            try
            {
                var response = await next();

                logger.LogTrace("Request executed with {@Response}", response);

                return response;
            }
            catch (ApiException e)
            {
                logger.LogDebug(e, "Request execution failed");

                throw;
            }
            catch (Exception e)
            {
                logger.LogError("Request execution failed due to {Message}", e.Message);
                logger.LogDebug(e, "Request execution failed");

                throw;
            }
        }
    }
}