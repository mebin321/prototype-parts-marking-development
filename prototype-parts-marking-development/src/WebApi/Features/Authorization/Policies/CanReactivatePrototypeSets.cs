namespace WebApi.Features.Authorization.Policies
{
    using Microsoft.AspNetCore.Authorization;

    public class CanReactivatePrototypeSets : IAuthorizationRequirement
    {
        public class Handler : PermissionHandler<CanReactivatePrototypeSets>
        {
            public Handler()
                : base(RoleName.Admin, RoleName.SuperAdmin)
            {
            }
        }
    }
}