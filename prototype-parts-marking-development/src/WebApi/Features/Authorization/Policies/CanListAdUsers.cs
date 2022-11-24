namespace WebApi.Features.Authorization.Policies
{
    using Microsoft.AspNetCore.Authorization;

    public class CanListAdUsers : IAuthorizationRequirement
    {
        public class Handler : PermissionHandler<CanListAdUsers>
        {
            public Handler()
                : base(RoleName.Admin, RoleName.SuperAdmin)
            {
            }
        }
    }
}