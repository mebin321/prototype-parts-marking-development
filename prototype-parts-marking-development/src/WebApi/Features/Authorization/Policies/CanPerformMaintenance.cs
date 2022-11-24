namespace WebApi.Features.Authorization.Policies
{
    using Microsoft.AspNetCore.Authorization;

    public class CanPerformMaintenance : IAuthorizationRequirement
    {
        public class Handler : PermissionHandler<CanPerformMaintenance>
        {
            public Handler()
                : base(RoleName.SuperAdmin)
            {
            }
        }
    }
}