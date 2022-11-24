namespace WebApi.Features.Authorization.Policies
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Utilities;

    public abstract class PermissionHandler<T> : AuthorizationHandler<T>, IPermissionHandler
        where T : IAuthorizationRequirement
    {
        private readonly HashSet<string> allowedRoles;

        protected PermissionHandler(params string[] allowedRoles)
        {
            Guard.NotNull(allowedRoles, nameof(allowedRoles));

            this.allowedRoles = new HashSet<string>(allowedRoles);
        }

        public string Name => typeof(T).Name;

        public bool AllowedFor(string role) => allowedRoles.Contains(role);

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, T requirement)
        {
            foreach (var claim in context.User.Claims.Where(c => c.Type == ClaimTypes.Role))
            {
                if (allowedRoles.Contains(claim.Value))
                {
                    context.Succeed(requirement);
                    return Task.CompletedTask;
                }
            }

            return Task.CompletedTask;
        }
    }
}