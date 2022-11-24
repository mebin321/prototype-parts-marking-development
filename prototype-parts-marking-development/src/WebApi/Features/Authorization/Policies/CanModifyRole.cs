namespace WebApi.Features.Authorization.Policies
{
    using Microsoft.AspNetCore.Authorization;

    public class CanModifyRole : IAuthorizationRequirement
    {
        public class Handler : PermissionHandler<CanModifyRole>
        {
            public Handler()
                : base(RoleName.SuperAdmin)
            {
            }
        }
    }
}