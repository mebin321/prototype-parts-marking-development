namespace WebApi.Features.Authorization.Policies
{
    using Microsoft.AspNetCore.Authorization;

    public class CanReactivatePrototypes : IAuthorizationRequirement
    {
        public class Handler : PermissionHandler<CanReactivatePrototypes>
        {
            public Handler()
                : base(RoleName.Admin, RoleName.SuperAdmin)
            {
            }
        }
    }
}