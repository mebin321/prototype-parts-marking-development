namespace WebApi.Features.Authorization.Policies
{
    using Microsoft.AspNetCore.Authorization;

    public class CanEnableUsers : IAuthorizationRequirement
    {
        public class Handler : PermissionHandler<CanEnableUsers>
        {
            public Handler()
                : base(RoleName.Admin, RoleName.SuperAdmin)
            {
            }
        }
    }
}