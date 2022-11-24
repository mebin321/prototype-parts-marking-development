namespace WebApi.Features.Authorization.Policies
{
    using Microsoft.AspNetCore.Authorization;

    public class CanDisableUsers : IAuthorizationRequirement
    {
        public class Handler : PermissionHandler<CanDisableUsers>
        {
            public Handler()
                : base(
                    RoleName.Coordinator,
                    RoleName.ApplicationEngineer,
                    RoleName.FixturesTechnician,
                    RoleName.Admin,
                    RoleName.SuperAdmin)
            {
            }
        }
    }
}