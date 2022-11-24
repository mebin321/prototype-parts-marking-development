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
    using WebApi.Features.Authorization.Policies;
    using WebApi.Features.Users.Models;
    using WebApi.Features.Users.Requests;

    [Authorize]
    [ApiController]
    [Route("api/v1")]
    public class UsersController : ControllerBase
    {
        private readonly IMediator mediator;

        public UsersController(IMediator mediator)
        {
            Guard.NotNull(mediator, nameof(mediator));

            this.mediator = mediator;
        }

        /// <summary>
        ///     Retrieve a specific User.
        /// </summary>
        [AllowAnonymous]
        [HttpGet("users/{userId}", Name = nameof(GetUser))]
        [Produces(MediaTypes.ApplicationJson)]
        [SwaggerResponse(StatusCodes.Status200OK, "User has been retrieved.", typeof(EnrichedUserDto))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid Id value.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "User with the provided Id does not exist.")]
        public async Task<ActionResult<EnrichedUserDto>> GetUser(int userId)
        {
            return await mediator.Send(new GetUserQuery { UserId = userId });
        }

        /// <summary>
        ///     Retrieve a list of Users.
        /// </summary>
        /// <remarks>
        ///     The 'search' parameter performs a full text search
        ///     on the 'DomainIdentity' and 'Email' fields.
        /// </remarks>
        [AllowAnonymous]
        [HttpGet("users", Name = nameof(ListUsers))]
        [Produces(MediaTypes.ApplicationJson)]
        [SwaggerResponse(StatusCodes.Status200OK, "Users have been retrieved.", typeof(PagedDataDto<EnrichedUserDto>))]
        public async Task<ActionResult<PagedDataDto<EnrichedUserDto>>> ListUsers([FromQuery] ListUsersQuery query)
        {
            return await mediator.Send(query);
        }

        /// <summary>
        ///     Create a new User from active directories.
        /// </summary>
        [Authorize(Policy = nameof(CanModifyUsers))]
        [HttpPost("users", Name = nameof(CreateUser))]
        [Produces(MediaTypes.ApplicationJson)]
        [Consumes(MediaTypes.ApplicationJson)]
        [SwaggerResponse(StatusCodes.Status201Created, "User has been created.", typeof(UserDto))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request model.")]
        public async Task<ActionResult<UserDto>> CreateUser(CreateUserCommand command)
        {
            var user = await mediator.Send(command);

            return CreatedAtRoute(nameof(GetUser), new { userId = user.Id }, user);
        }

        /// <summary>
        ///     Update an existing User.
        /// </summary>
        [Authorize(Policy = nameof(CanModifyUsers))]
        [HttpPut("users/{userId}", Name = nameof(UpdateUser))]
        [Produces(MediaTypes.ApplicationJson)]
        [Consumes(MediaTypes.ApplicationJson)]
        [SwaggerResponse(StatusCodes.Status200OK, "User has been updated.", typeof(UserDto))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request model.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "User with the provided Id does not exist.")]
        public async Task<ActionResult<UserDto>> UpdateUser(int userId, UpdateUserCommandDto command)
        {
            return await mediator.Send(new UpdateUserCommand
            {
                UserId = userId,
                Name = command.Name,
                Email = command.Email,
            });
        }

        /// <summary>
        ///     Disable an active User.
        /// </summary>
        [Authorize(Policy = nameof(CanDisableUsers))]
        [HttpDelete("active-users/{userId}", Name = nameof(DeleteUser))]
        [SwaggerResponse(StatusCodes.Status204NoContent, "User has been disabled.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "User with provided Id does not exist.")]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            await mediator.Send(new DeleteUserCommand { UserId = userId });

            return NoContent();
        }

        /// <summary>
        ///     Enable an inactive User.
        /// </summary>
        [Authorize(Policy = nameof(CanEnableUsers))]
        [HttpDelete("inactive-users/{userId}", Name = nameof(ReactivateUser))]
        [Produces(MediaTypes.ApplicationJson)]
        [SwaggerResponse(StatusCodes.Status204NoContent, "User has been enabled.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "User with provided Id does not exist.")]
        public async Task<IActionResult> ReactivateUser(int userId)
        {
            await mediator.Send(new ReactivateUserCommand { UserId = userId });

            return NoContent();
        }
    }
}
