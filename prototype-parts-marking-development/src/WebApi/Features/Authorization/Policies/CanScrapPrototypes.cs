namespace WebApi.Features.Authorization.Policies
{
    using Microsoft.AspNetCore.Authorization;

    public class CanScrapPrototypes : IAuthorizationRequirement
    {
        public class Handler : PermissionHandler<CanScrapPrototypes>
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