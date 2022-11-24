namespace WebApi
{
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Infrastructure;
    using Microsoft.AspNetCore.Mvc.ModelBinding;
    using Utilities;

    public class ProblemDetailsAdapterFactory : IProblemDetailsFactory
    {
        private readonly ProblemDetailsFactory problemDetailsFactory;
        private readonly IHttpContextAccessor contextAccessor;

        public ProblemDetailsAdapterFactory(ProblemDetailsFactory problemDetailsFactory, IHttpContextAccessor contextAccessor)
        {
            Guard.NotNull(problemDetailsFactory, nameof(problemDetailsFactory));
            Guard.NotNull(contextAccessor, nameof(contextAccessor));

            this.problemDetailsFactory = problemDetailsFactory;
            this.contextAccessor = contextAccessor;
        }

        public ProblemDetails Create(int statusCode, string title, string detail)
        {
            var context = contextAccessor.HttpContext;

            return problemDetailsFactory.CreateProblemDetails(
                context,
                statusCode,
                title,
                null,
                detail,
                context.Request.Path);
        }

        public ValidationProblemDetails CreateValidationProblemDetails(Dictionary<string, string[]> errors)
        {
            var context = contextAccessor.HttpContext;
            var modelState = new ModelStateDictionary();
            foreach (var (key, value) in errors)
            {
                foreach (var error in value)
                {
                    modelState.AddModelError(key, error);
                }
            }

            return problemDetailsFactory.CreateValidationProblemDetails(
                context,
                modelState,
                StatusCodes.Status400BadRequest,
                null,
                null,
                null,
                context.Request.Path);
        }
    }
}