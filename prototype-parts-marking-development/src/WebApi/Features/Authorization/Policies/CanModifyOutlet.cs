namespace WebApi.Features.Authorization.Policies
{
    using Microsoft.AspNetCore.Authorization;

    public class CanModifyOutlet : IAuthorizationRequirement
    {
        public class Handler : PermissionHandler<CanModifyOutlet>
        {
            public Handler()
                : base(RoleName.Admin, RoleName.SuperAdmin)
            {
            }
        }
    }
}