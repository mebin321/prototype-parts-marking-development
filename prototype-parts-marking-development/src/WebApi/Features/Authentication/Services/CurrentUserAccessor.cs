namespace WebApi.Features.Authentication.Services
{
    using Microsoft.AspNetCore.Http;
    using Utilities;

    public class CurrentUserAccessor : ICurrentUserAccessor
    {
        private readonly IHttpContextAccessor contextAccessor;
        private readonly IProblemDetailsFactory problemDetailsFactory;

        public CurrentUserAccessor(IHttpContextAccessor contextAccessor, IProblemDetailsFactory problemDetailsFactory)
        {
            Guard.NotNull(contextAccessor, nameof(contextAccessor));
            Guard.NotNull(problemDetailsFactory, nameof(problemDetailsFactory));

            this.contextAccessor = contextAccessor;
            this.problemDetailsFactory = problemDetailsFactory;
        }

        public int GetCurrentUser()
        {
            var claim = contextAccessor.HttpContext?.User?.FindFirst("User");

            return int.TryParse(claim?.Value, out var result)
                ? result
                : throw new BadRequestException(problemDetailsFactory.BadRequest(
                    "Missing User ID",
                    "Token does not contain User ID."));
        }
    }
}