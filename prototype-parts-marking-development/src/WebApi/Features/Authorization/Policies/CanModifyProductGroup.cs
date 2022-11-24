namespace WebApi.Features.Authorization.Policies
{
    using Microsoft.AspNetCore.Authorization;

    public class CanModifyProductGroup : IAuthorizationRequirement
    {
        public class Handler : PermissionHandler<CanModifyProductGroup>
        {
            public Handler()
                : base(RoleName.Admin, RoleName.SuperAdmin)
            {
            }
        }
    }
}