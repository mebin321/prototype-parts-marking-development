namespace WebApi.Features.Roles.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using MediatR;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Swashbuckle.AspNetCore.Annotations;
    using Utilities;
    using WebApi.Features.Authorization.Policies;
    using WebApi.Features.Roles.Models;
    using WebApi.Features.Roles.Requests;

    [Authorize]
    [ApiController]
    [Route("api/v1/users")]
    public class UserRolesController : ControllerBase
    {
        private readonly IMediator mediator;

        public UserRolesController(IMediator mediator)
        {
            Guard.NotNull(mediator, nameof(mediator));

            this.mediator = mediator;
        }

        /// <summary>
        ///     Retrieve a list of Roles assigned to a specific User.
        /// </summary>
        [AllowAnonymous]
        [HttpGet("{userId}/roles")]
        [SwaggerOperation(Tags = new[] { "Users" })]
        [Produces(MediaTypes.ApplicationJson)]
        [SwaggerResponse(StatusCodes.Status200OK, "Roles assigned to a specific User have been retrieved.", typeof(RoleDto))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "User with the provided Id does not exist.")]
        public async Task<ActionResult<List<RoleDto>>> ListUserRoles([FromQuery] ListUserRolesQuery query)
        {
            return await mediator.Send(query);
        }

        /// <summary>
        ///     Assign Role to a User.
        /// </summary>
        [Authorize(Policy = nameof(CanModifyUserRole))]
        [HttpPost("{userId}/roles")]
        [SwaggerOperation(Tags = new[] { "Users" })]
        [Produces(MediaTypes.ApplicationJson)]
        [SwaggerResponse(StatusCodes.Status200OK, "Role has been assigned to a User.", typeof(List<RoleDto>))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "User with the provided Id or Role with provided moniker does not exist.")]
        public async Task<ActionResult<List<RoleDto>>> AddUserRole(int userId, UserRoleRequestDto dto)
        {
            return await mediator.Send(new AddUserRoleCommand
            {
                UserId = userId,
                RoleMoniker = dto.Moniker,
            });
        }

        /// <summary>
        ///     Unassign Role from a User.
        /// </summary>
        [Authorize(Policy = nameof(CanModifyUserRole))]
        [HttpDelete("{userId}/roles/{roleMoniker}")]
        [SwaggerOperation(Tags = new[] { "Users" })]
        [Produces(MediaTypes.ApplicationJson)]
        [SwaggerResponse(StatusCodes.Status200OK, "Role has been unassigned from a User.", typeof(List<RoleDto>))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "User with the provided Id does not exist.")]
        public async Task<ActionResult<List<RoleDto>>> RemoveUserRole(int userId, string roleMoniker)
        {
            return await mediator.Send(new RemoveUserRoleCommand
            {
                UserId = userId,
                RoleMoniker = roleMoniker,
            });
        }

        /// <summary>
        ///     Update Roles assigned to a specific User.
        /// </summary>
        [Authorize(Policy = nameof(CanModifyUserRole))]
        [HttpPut("{userId}/roles")]
        [SwaggerOperation(Tags = new[] { "Users" })]
        [Produces(MediaTypes.ApplicationJson)]
        [Consumes(MediaTypes.ApplicationJson)]
        [SwaggerResponse(StatusCodes.Status200OK, "Roles assigned to a specific User have been updated.", typeof(List<RoleDto>))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request model.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "User with the provided Id does not exist.")]

        public async Task<ActionResult<List<RoleDto>>> UpdateUserRoles(int userId, List<UserRoleRequestDto> roles)
        {
            return await mediator.Send(new UpdateUserRolesCommand
            {
                UserId = userId,
                RoleMonikers = roles.Select(r => r.Moniker).ToList(),
            });
        }
    }
}
