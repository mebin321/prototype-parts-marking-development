namespace WebApi.Features.Outlets
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
    [Route("api/v1/outlets")]
    public class OutletsController : ControllerBase
    {
        private readonly IMediator mediator;

        public OutletsController(IMediator mediator)
        {
            Guard.NotNull(mediator, nameof(mediator));

            this.mediator = mediator;
        }

        /// <summary>
        ///     Retrieve a specific Outlet.
        /// </summary>
        [AllowAnonymous]
        [HttpGet("{moniker}", Name = nameof(GetOutlet))]
        [Produces(MediaTypes.ApplicationJson)]
        [SwaggerResponse(StatusCodes.Status200OK, "Outlet has been retrieved.", typeof(OutletDto))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid moniker value.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Outlet with the provided moniker does not exist.")]
        public async Task<ActionResult<OutletDto>> GetOutlet(string moniker)
        {
            return await mediator.Send(new GetOutletQuery { Moniker = moniker });
        }

        /// <summary>
        ///     Retrieve a list of Outlets.
        /// </summary>
        /// <remarks>
        ///     The 'search' parameter performs a full text search
        ///     on the 'Title' and 'Description' fields.
        /// </remarks>
        [AllowAnonymous]
        [HttpGet(Name = nameof(ListOutlets))]
        [Produces(MediaTypes.ApplicationJson)]
        [SwaggerResponse(StatusCodes.Status200OK, "Outlets have been retrieved.", typeof(PagedDataDto<OutletDto>))]
        public async Task<ActionResult<PagedDataDto<OutletDto>>> ListOutlets([FromQuery] ListOutletsQuery query)
        {
            return await mediator.Send(query);
        }

        /// <summary>
        ///     Create a new Outlet.
        /// </summary>
        [Authorize(Policy = nameof(CanModifyOutlet))]
        [HttpPost(Name = nameof(CreateOutlet))]
        [Produces(MediaTypes.ApplicationJson)]
        [Consumes(MediaTypes.ApplicationJson)]
        [SwaggerResponse(StatusCodes.Status201Created, "Outlet has been created.", typeof(OutletDto))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request model.")]
        public async Task<ActionResult<OutletDto>> CreateOutlet(CreateOutletCommand command)
        {
            var outlet = await mediator.Send(command);

            return CreatedAtRoute(nameof(GetOutlet), new { outlet.Moniker }, outlet);
        }

        /// <summary>
        ///     Update an existing Outlet.
        /// </summary>
        [Authorize(Policy = nameof(CanModifyOutlet))]
        [HttpPut("{moniker}", Name = nameof(UpdateOutlet))]
        [Produces(MediaTypes.ApplicationJson)]
        [Consumes(MediaTypes.ApplicationJson)]
        [SwaggerResponse(StatusCodes.Status200OK, "Outlet has been updated.", typeof(OutletDto))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request model.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Outlet with the provided moniker does not exist.")]
        public async Task<ActionResult<OutletDto>> UpdateOutlet(string moniker, UpdateOutletRequestDto request)
        {
            return await mediator.Send(new UpdateOutletCommand
            {
                Moniker = moniker,
                Description = request.Description,
            });
        }
    }
}
