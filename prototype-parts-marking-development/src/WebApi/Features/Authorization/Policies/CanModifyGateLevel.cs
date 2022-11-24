namespace WebApi.Features.Authorization.Policies
{
    using Microsoft.AspNetCore.Authorization;

    public class CanModifyGateLevel : IAuthorizationRequirement
    {
        public class Handler : PermissionHandler<CanModifyGateLevel>
        {
            public Handler()
                : base(RoleName.Admin, RoleName.SuperAdmin)
            {
            }
        }
    }
}