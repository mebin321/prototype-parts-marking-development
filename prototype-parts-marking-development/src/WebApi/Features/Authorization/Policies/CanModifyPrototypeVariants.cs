namespace WebApi.Features.Authorization.Policies
{
    using Microsoft.AspNetCore.Authorization;

    public class CanModifyPrototypeVariants : IAuthorizationRequirement
    {
        public class Handler : PermissionHandler<CanModifyPrototypeVariants>
        {
            public Handler()
                : base(
                    RoleName.TestEngineer,
                    RoleName.Coordinator,
                    RoleName.ApplicationEngineer,
                    RoleName.Tpm,
                    RoleName.TestTechnician,
                    RoleName.Admin,
                    RoleName.SuperAdmin)
            {
            }
        }
    }
}