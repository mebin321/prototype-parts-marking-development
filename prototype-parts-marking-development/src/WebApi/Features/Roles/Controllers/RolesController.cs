namespace WebApi.Features.Roles.Controllers
{
    using System.Linq;
    using System.Threading.Tasks;
    using MediatR;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.DependencyInjection;
    using Swashbuckle.AspNetCore.Annotations;
    using Utilities;
    using WebApi.Common.Paging;
    using WebApi.Features.Authorization;
    using WebApi.Features.Authorization.Policies;
    using WebApi.Features.Roles.Models;
    using WebApi.Features.Roles.Requests;

    [Authorize]
    [ApiController]
    [Route("api/v1/roles")]
    public class RolesController : ControllerBase
    {
        private readonly IMediator mediator;

        public RolesController(IMediator mediator)
        {
            Guard.NotNull(mediator, nameof(mediator));

            this.mediator = mediator;
        }

        /// <summary>
        ///     Retrieve a specific Role.
        /// </summary>
        [AllowAnonymous]
        [HttpGet("{moniker}", Name = nameof(GetRole))]
        [Produces(MediaTypes.ApplicationJson)]
        [SwaggerResponse(StatusCodes.Status200OK, "Role has been retrieved.", typeof(RoleDto))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid moniker value.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Role with the provided moniker does not exist.")]
        public async Task<ActionResult<RoleDto>> GetRole(string moniker)
        {
            return await mediator.Send(new GetRoleQuery { Moniker = moniker });
        }

        /// <summary>
        ///     Retrieve a list of Roles.
        /// </summary>
        /// <remarks>
        ///     The 'search' parameter performs a full text search
        ///     on the 'Title' and 'Description' fields.
        /// </remarks>
        [AllowAnonymous]
        [HttpGet(Name = nameof(ListRoles))]
        [Produces(MediaTypes.ApplicationJson)]
        [SwaggerResponse(StatusCodes.Status200OK, "Roles have been retrieved.", typeof(PagedDataDto<RoleDto>))]
        public async Task<ActionResult<PagedDataDto<RoleDto>>> ListRoles([FromQuery] ListRolesQuery query)
        {
            return await mediator.Send(query);
        }

        /// <summary>
        ///     Create a new Role.
        /// </summary>
        [Authorize(Policy = nameof(CanModifyRole))]
        [HttpPost(Name = nameof(CreateRole))]
        [Produces(MediaTypes.ApplicationJson)]
        [Consumes(MediaTypes.ApplicationJson)]
        [SwaggerResponse(StatusCodes.Status201Created, "Role has been created.", typeof(RoleDto))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request model.")]
        public async Task<ActionResult<RoleDto>> CreateRole(CreateRoleCommand command)
        {
            var role = await mediator.Send(command);

            return CreatedAtRoute(nameof(GetRole), new { role.Moniker }, role);
        }

        /// <summary>
        ///     Update an existing Role.
        /// </summary>
        [Authorize(Policy = nameof(CanModifyRole))]
        [HttpPut("{moniker}", Name = nameof(UpdateRole))]
        [Produces(MediaTypes.ApplicationJson)]
        [Consumes(MediaTypes.ApplicationJson)]
        [SwaggerResponse(StatusCodes.Status200OK, "Role has been updated.", typeof(RoleDto))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request model.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Role with the provided moniker does not exist.")]
        public async Task<ActionResult<RoleDto>> UpdateRole(string moniker, UpdateRoleRequestDto dto)
        {
            return await mediator.Send(new UpdateRoleCommand
            {
                Moniker = moniker,
                Title = dto.Title,
                Description = dto.Description,
            });
        }

        /// <summary>
        ///     Delete a Role.
        /// </summary>
        [Authorize(Policy = nameof(CanModifyRole))]
        [HttpDelete("{moniker}", Name = nameof(DeleteRole))]
        [Produces(MediaTypes.ApplicationJson)]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Role has been deleted.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid moniker value.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Role with the provided moniker does not exist.")]
        public async Task<IActionResult> DeleteRole(string moniker)
        {
            await mediator.Send(new DeleteRoleCommand { Moniker = moniker });

            return NoContent();
        }

        /// <summary>
        ///     Retrieve Role resource metadata.
        /// </summary>
        [AllowAnonymous]
        [HttpOptions]
        [Produces(MediaTypes.ApplicationJson)]
        public IActionResult RolesMetadata()
        {
            var handlers = HttpContext.RequestServices
                .GetServices<IAuthorizationHandler>()
                .OfType<IPermissionHandler>()
                .ToList();

            var map = RoleName.ListRoles().Select(r => new
            {
                Moniker = r,
                Permissions = handlers.Where(h => h.AllowedFor(r)).Select(h => h.Name),
            });

            return Ok(new { PermissionMap = map });
        }
    }
}