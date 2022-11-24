namespace WebApi.Common.ResourceVersioning
{
    using System;
    using System.Net.Http.Headers;
    using Data;
    using Microsoft.AspNetCore.Http;
    using Utilities;

    public class ResourceVersionManager : IResourceVersionManager
    {
        private const string ETagHeaderName = "ETag";

        private const string IfMatchHeaderName = "If-Match";

        private readonly IHttpContextAccessor httpContextAccessor;

        private readonly IProblemDetailsFactory problemDetailsFactory;

        private readonly IETagGenerator eTagGenerator;

        public ResourceVersionManager(
            IHttpContextAccessor httpContextAccessor,
            IProblemDetailsFactory problemDetailsFactory,
            IETagGenerator eTagGenerator)
        {
            Guard.NotNull(httpContextAccessor, nameof(httpContextAccessor));
            Guard.NotNull(problemDetailsFactory, nameof(problemDetailsFactory));
            Guard.NotNull(eTagGenerator, nameof(eTagGenerator));

            this.httpContextAccessor = httpContextAccessor;
            this.problemDetailsFactory = problemDetailsFactory;
            this.eTagGenerator = eTagGenerator;
        }

        public void CheckVersion(IAuditableEntity entity, bool allowWildcard)
        {
            var context = httpContextAccessor.HttpContext ?? throw new InvalidOperationException("Missing HTTP context.");
            if (context.Request.Headers.TryGetValue(IfMatchHeaderName, out var result))
            {
                var receivedEtag = result[0].Trim('"');

                if (allowWildcard && receivedEtag == EntityTagHeaderValue.Any.Tag)
                {
                    return;
                }

                if (receivedEtag == eTagGenerator.ETagFrom(entity))
                {
                    return;
                }
            }

            throw new PreconditionFailedException(
                problemDetailsFactory.PreconditionFailed(
                    "Precondition failed.",
                    "Provided ETag does not match the current Resource version."));
        }

        public void SetEtag(IAuditableEntity entity)
        {
            var etag = $"\"{eTagGenerator.ETagFrom(entity)}\"";
            httpContextAccessor.HttpContext?.Response.Headers.Add(ETagHeaderName, etag);
        }
    }
}
