namespace WebApi.MediatrPipeline
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using FluentValidation;
    using MediatR;
    using Utilities;

    public class RequestValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IValidator<TRequest>[] validators;
        private readonly IProblemDetailsFactory problemDetailsFactory;

        public RequestValidationBehavior(IEnumerable<IValidator<TRequest>> validators, IProblemDetailsFactory problemDetailsFactory)
        {
            Guard.NotNull(validators, nameof(validators));
            Guard.NotNull(problemDetailsFactory, nameof(problemDetailsFactory));

            this.validators = validators.ToArray();
            this.problemDetailsFactory = problemDetailsFactory;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            var failures = validators
                .Select(v => v.Validate(request))
                .SelectMany(r => r.Errors)
                .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
                .ToDictionary(g => g.Key, g => g.ToArray());

            if (failures.Count > 0)
            {
                throw new ModelValidationFailedException(problemDetailsFactory.CreateValidationProblemDetails(failures));
            }

            return await next();
        }
    }
}