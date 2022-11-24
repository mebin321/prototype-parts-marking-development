namespace WebApi.Features.Authentication
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
    using WebApi.Features.Authentication.Models;
    using WebApi.Features.Authentication.Requests;
    using WebApi.Features.Users.Models;

    [Authorize]
    [ApiController]
    [Route("api/v1/auth")]
    public sealed class AuthenticationController : ControllerBase
    {
        private readonly IMediator mediator;

        public AuthenticationController(IMediator mediator)
        {
            Guard.NotNull(mediator, nameof(mediator));

            this.mediator = mediator;
        }

        /// <summary>
        ///     Sign in using the provided username and password.
        /// </summary>
        /// <remarks>
        ///     Non-interactive (service) accounts may not authenticate using this endpoint.
        /// </remarks>
        [AllowAnonymous]
        [HttpPost("authenticate", Name = nameof(Authenticate))]
        [Produces(MediaTypes.ApplicationJson)]
        [Consumes(MediaTypes.ApplicationJson)]
        [SwaggerResponse(StatusCodes.Status201Created, "User has been authenticated.", typeof(AuthenticationResponseDto))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request model.")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Invalid username or password.")]
        public async Task<ActionResult<AuthenticationResponseDto>> Authenticate(AuthenticateCommand command)
        {
            return await mediator.Send(command);
        }

        /// <summary>
        ///     Refresh access token using the provided refesh token.
        /// </summary>
        [AllowAnonymous]
        [HttpPost("refresh-access-token", Name = nameof(RefreshAccessToken))]
        [Produces(MediaTypes.ApplicationJson)]
        [Consumes(MediaTypes.ApplicationJson)]
        [SwaggerResponse(StatusCodes.Status201Created, "Access token has been refreshed.", typeof(AuthenticationResponseDto))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request model.")]
        public async Task<ActionResult<AuthenticationResponseDto>> RefreshAccessToken(RefreshAccessTokenCommand command)
        {
            return await mediator.Send(command);
        }

        /// <summary>
        ///     Retrieve information regaring the currently signed in user.
        /// </summary>
        [Authorize]
        [HttpGet("current-user", Name = nameof(CurrentUser))]
        [Produces(MediaTypes.ApplicationJson)]
        [SwaggerResponse(StatusCodes.Status200OK, "Currently signed User has been retrieved.", typeof(UserDto))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "User does not exist.")]
        public async Task<ActionResult<UserDto>> CurrentUser()
        {
            return await mediator.Send(new CurrentUserQuery());
        }

        /// <summary>
        ///     Retrieve claims held by the currently signed in user.
        /// </summary>
        [Authorize]
        [HttpGet("current-claims", Name = nameof(CurrentClaims))]
        [Produces(MediaTypes.ApplicationJson)]
        [SwaggerResponse(StatusCodes.Status200OK, "Claims has been retrieved.", typeof(IEnumerable<object>))]
        public IActionResult CurrentClaims()
        {
            var claims = User.Claims.Select(c => new
            {
                c.Value,
                c.Type,
                c.Issuer,
                c.ValueType,
            });

            return Ok(claims);
        }
    }
}
