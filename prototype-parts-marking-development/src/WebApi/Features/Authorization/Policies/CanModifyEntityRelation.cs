namespace WebApi.Features.Authorization.Policies
{
    using Microsoft.AspNetCore.Authorization;

    public class CanModifyEntityRelation : IAuthorizationRequirement
    {
        public class Handler : PermissionHandler<CanModifyEntityRelation>
        {
            public Handler()
                : base(RoleName.Admin, RoleName.SuperAdmin)
            {
            }
        }
    }
}