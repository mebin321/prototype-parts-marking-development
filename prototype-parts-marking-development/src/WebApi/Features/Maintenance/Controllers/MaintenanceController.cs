namespace WebApi.Features.Maintenance.Controllers
{
    using System.Threading.Tasks;
    using MediatR;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Swashbuckle.AspNetCore.Annotations;
    using Utilities;
    using WebApi.Features.Authorization.Policies;

    [Authorize]
    [ApiController]
    [Route("api/v1/maintenance")]
    public class MaintenanceController : ControllerBase
    {
        private readonly IMediator mediator;

        public MaintenanceController(IMediator mediator)
        {
            Guard.NotNull(mediator, nameof(mediator));

            this.mediator = mediator;
        }

        /// <summary>
        ///     Revoke a specific refresh token.
        /// </summary>
        [Authorize(Policy = nameof(CanPerformMaintenance))]
        [HttpPost("revoke-refresh-token", Name = nameof(RevokeRefreshToken))]
        [Consumes(MediaTypes.ApplicationJson)]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Token is revoked.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request model.")]
        public async Task<IActionResult> RevokeRefreshToken(RevokeRefreshTokenCommand command)
        {
            await mediator.Send(command);
            return NoContent();
        }
    }
}
