namespace WebApi.Features.Authorization.Policies
{
    using Microsoft.AspNetCore.Authorization;

    public class CanModifyEvidenceYear : IAuthorizationRequirement
    {
        public class Handler : PermissionHandler<CanModifyEvidenceYear>
        {
            public Handler()
                : base(RoleName.Admin, RoleName.SuperAdmin)
            {
            }
        }
    }
}