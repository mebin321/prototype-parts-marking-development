namespace WebApi.Features.Authorization.Policies
{
    using Microsoft.AspNetCore.Authorization;

    public class CanModifyUsers : IAuthorizationRequirement
    {
        public class Handler : PermissionHandler<CanModifyUsers>
        {
            public Handler()
                : base(RoleName.Admin, RoleName.SuperAdmin)
            {
            }
        }
    }
}