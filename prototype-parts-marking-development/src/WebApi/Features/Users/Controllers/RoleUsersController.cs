namespace WebApi.Features.Users.Controllers
{
    using System.Threading.Tasks;
    using MediatR;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Swashbuckle.AspNetCore.Annotations;
    using Utilities;
    using WebApi.Common.Paging;
    using WebApi.Features.Users.Models;
    using WebApi.Features.Users.Requests;

    [Authorize]
    [ApiController]
    [Route("api/v1/roles")]
    public class RoleUsersController : ControllerBase
    {
        private readonly IMediator mediator;

        public RoleUsersController(IMediator mediator)
        {
            Guard.NotNull(mediator, nameof(mediator));

            this.mediator = mediator;
        }

        /// <summary>
        ///     Retrieve a list of Users assigned to a specific Role.
        /// </summary>
        [AllowAnonymous]
        [HttpGet("{moniker}/users")]
        [SwaggerOperation(Tags = new[] { "Roles" })]
        [Produces(MediaTypes.ApplicationJson)]
        [SwaggerResponse(StatusCodes.Status200OK, "users assigned to a specific Role have been retrieved.", typeof(PagedDataDto<UserDto>))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid moniker value.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Role with provided Moniker does not exist.")]
        public async Task<ActionResult<PagedDataDto<UserDto>>> ListRoleUsers([FromQuery] ListRoleUsersQuery query)
        {
            return await mediator.Send(query);
        }
    }
}