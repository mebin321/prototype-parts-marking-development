namespace WebApi.Features.Authorization.Policies
{
    using Microsoft.AspNetCore.Authorization;

    public class CanScrapPrototypeSets : IAuthorizationRequirement
    {
        public class Handler : PermissionHandler<CanScrapPrototypeSets>
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