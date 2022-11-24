namespace WebApi.Features.Authorization.Policies
{
    using Microsoft.AspNetCore.Authorization;

    public class CanReactivatePrototypesPackages : IAuthorizationRequirement
    {
        public class Handler : PermissionHandler<CanReactivatePrototypesPackages>
        {
            public Handler()
                : base(RoleName.Admin, RoleName.SuperAdmin)
            {
            }
        }
    }
}