namespace WebApi.MediatrPipeline
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using Microsoft.Extensions.Logging;
    using Utilities;

    public class LoggingContextBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly ILogger<LoggingContextBehavior<TRequest, TResponse>> logger;

        public LoggingContextBehavior(ILogger<LoggingContextBehavior<TRequest, TResponse>> logger)
        {
            Guard.NotNull(logger, nameof(logger));

            this.logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            var metadata = new[]
            {
                new KeyValuePair<string, object>("RequestCorrelationId", Guid.NewGuid().ToString()),
                new KeyValuePair<string, object>("RequestType", typeof(TRequest).FullName),
            };

            using (logger.BeginScope(metadata))
            {
                return await next();
            }
        }
    }
}