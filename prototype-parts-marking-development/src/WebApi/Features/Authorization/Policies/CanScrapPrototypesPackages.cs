namespace WebApi.Features.Authorization.Policies
{
    using Microsoft.AspNetCore.Authorization;

    public class CanScrapPrototypesPackages : IAuthorizationRequirement
    {
        public class Handler : PermissionHandler<CanScrapPrototypesPackages>
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