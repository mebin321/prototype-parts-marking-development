namespace WebApi.Features.AdUsers.Controllers
{
    using System.Threading.Tasks;
    using Common.Paging;
    using MediatR;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using Requests;
    using Swashbuckle.AspNetCore.Annotations;
    using Utilities;
    using WebApi.Features.Authorization.Policies;

    [Authorize]
    [ApiController]
    [Route("api/v1/adusers")]
    public class AdUsersController : ControllerBase
    {
        private readonly IMediator mediator;

        public AdUsersController(IMediator mediator)
        {
            Guard.NotNull(mediator, nameof(mediator));

            this.mediator = mediator;
        }

        /// <summary>
        ///     Search the Active Directory for users based on query parameters.
        /// </summary>
        /// <remarks>
        ///     Only the first 100 results returned from each AD are processed.
        /// </remarks>
        [Authorize(Policy = nameof(CanListAdUsers))]
        [HttpGet(Name = nameof(ListAdUsers))]
        [Produces(MediaTypes.ApplicationJson)]
        [SwaggerResponse(StatusCodes.Status200OK, "Users have been retrieved.", typeof(PagedDataDto<AdUserDto>))]
        public async Task<ActionResult<PagedDataDto<AdUserDto>>> ListAdUsers([FromQuery] ListAdUsersQuery query)
        {
            return await mediator.Send(query);
        }
    }
}
