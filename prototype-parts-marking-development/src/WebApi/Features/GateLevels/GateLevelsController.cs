namespace WebApi.Features.GateLevels
{
    using System.Threading.Tasks;
    using Common.Paging;
    using MediatR;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using Swashbuckle.AspNetCore.Annotations;
    using Utilities;
    using WebApi.Features.Authorization.Policies;
    using WebApi.Features.GateLevels.Requests;

    [Authorize]
    [ApiController]
    [Route("api/v1/gatelevels")]
    public class GateLevelsController : ControllerBase
    {
        private readonly IMediator mediator;

        public GateLevelsController(IMediator mediator)
        {
            Guard.NotNull(mediator, nameof(mediator));

            this.mediator = mediator;
        }

        /// <summary>
        ///     Retrieve a specific Gate Level.
        /// </summary>
        [AllowAnonymous]
        [HttpGet("{moniker}", Name = nameof(GetGatelevel))]
        [Produces(MediaTypes.ApplicationJson)]
        [SwaggerResponse(StatusCodes.Status200OK, "Gate Level has been retrieved.", typeof(GateLevelDto))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid moniker value.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Gate Level with the provided moniker does not exist.")]
        public async Task<ActionResult<GateLevelDto>> GetGatelevel(string moniker)
        {
            return await mediator.Send(new GetGateLevelQuery { Moniker = moniker });
        }

        /// <summary>
        ///     Retrieve a list of Gate Levels.
        /// </summary>
        [AllowAnonymous]
        [HttpGet(Name = nameof(ListGateLevels))]
        [Produces(MediaTypes.ApplicationJson)]
        [SwaggerResponse(StatusCodes.Status200OK, "Gate Levels have been retrieved.", typeof(PagedDataDto<GateLevelDto>))]
        public async Task<ActionResult<PagedDataDto<GateLevelDto>>> ListGateLevels([FromQuery] ListGateLevelsQuery query)
        {
            return await mediator.Send(query);
        }

        /// <summary>
        ///     Create a new Gate Level.
        /// </summary>
        [Authorize(Policy = nameof(CanModifyGateLevel))]
        [HttpPost(Name = nameof(CreateGateLevel))]
        [Produces(MediaTypes.ApplicationJson)]
        [Consumes(MediaTypes.ApplicationJson)]
        [SwaggerResponse(StatusCodes.Status201Created, "Gate Level has been created.", typeof(GateLevelDto))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request model.")]
        public async Task<ActionResult<GateLevelDto>> CreateGateLevel(CreateGateLevelCommand command)
        {
            var gateLevel = await mediator.Send(command);

            return CreatedAtRoute(nameof(GetGatelevel), new { gateLevel.Moniker }, gateLevel);
        }

        /// <summary>
        ///     Update an existing Gate Level.
        /// </summary>
        [Authorize(Policy = nameof(CanModifyGateLevel))]
        [HttpPut("{moniker}", Name = nameof(UpdateGateLevel))]
        [Produces(MediaTypes.ApplicationJson)]
        [Consumes(MediaTypes.ApplicationJson)]
        [SwaggerResponse(StatusCodes.Status200OK, "Gate Level has been updated.", typeof(GateLevelDto))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request model.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Gate Level with the provided moniker does not exist.")]
        public async Task<ActionResult<GateLevelDto>> UpdateGateLevel(string moniker, UpdateGateLevelRequestDto request)
        {
            return await mediator.Send(new UpdateGateLevelCommand
            {
                Moniker = moniker,
                Description = request.Description,
            });
        }
    }
}