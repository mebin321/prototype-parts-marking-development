namespace WebApi.Features.Authorization.Policies
{
    using Microsoft.AspNetCore.Authorization;

    public class CanModifyPrototypes : IAuthorizationRequirement
    {
        public class Handler : PermissionHandler<CanModifyPrototypes>
        {
            public Handler()
                : base(
                    RoleName.TestEngineer,
                    RoleName.Coordinator,
                    RoleName.ApplicationEngineer,
                    RoleName.Tpm,
                    RoleName.TestTechnician,
                    RoleName.FixturesTechnician,
                    RoleName.Admin,
                    RoleName.SuperAdmin)
            {
            }
        }
    }
}