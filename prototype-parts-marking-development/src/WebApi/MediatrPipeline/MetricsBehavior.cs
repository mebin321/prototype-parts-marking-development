namespace WebApi.MediatrPipeline
{
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using Microsoft.Extensions.Logging;
    using Utilities;
    using WebApi.Common.Metadata;

    public class MetricsBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly MetricsBehaviorOptions options;
        private readonly ILogger<MetricsBehavior<TRequest, TResponse>> logger;

        public MetricsBehavior(MetricsBehaviorOptions options, ILogger<MetricsBehavior<TRequest, TResponse>> logger)
        {
            Guard.NotNull(options, nameof(options));
            Guard.NotNull(logger, nameof(logger));

            this.options = options;
            this.logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            var threshold = MetadataCache<TRequest>
                .For<LongRunningRequestAttribute>
                .AttributeOrDefault?.Threshold ?? options.DefaultThreshold;

            var watch = Stopwatch.StartNew();
            try
            {
                return await next();
            }
            finally
            {
                watch.Stop();

                if (watch.Elapsed > threshold)
                {
                    logger.LogWarning("Anomalous request duration {Elapsed} {Threshold}", watch.Elapsed, threshold);
                }

                logger.LogTrace("Request processed in {Elapsed}", watch.Elapsed);
            }
        }
    }
}