namespace WebApi.Features.Locations
{
    using System.ComponentModel.DataAnnotations;
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
    [Route("api/v1/locations")]
    public class LocationsController : ControllerBase
    {
        private readonly IMediator mediator;

        public LocationsController(IMediator mediator)
        {
            Guard.NotNull(mediator, nameof(mediator));

            this.mediator = mediator;
        }

        /// <summary>
        ///     Retrieve a specific Location.
        /// </summary>
        [AllowAnonymous]
        [HttpGet("{moniker}", Name = nameof(GetLocation))]
        [Produces(MediaTypes.ApplicationJson)]
        [SwaggerResponse(StatusCodes.Status200OK, "Location has been retrieved.", typeof(LocationDto))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid moniker value.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Location with the provided moniker does not exist.")]
        public async Task<ActionResult<LocationDto>> GetLocation(
            [Required, SwaggerParameter("Location moniker.")] string moniker)
        {
            return await mediator.Send(new GetLocationQuery { Moniker = moniker });
        }

        /// <summary>
        ///     Retrieve a list of locations.
        /// </summary>
        /// <remarks>
        ///     The 'search' parameter performs a full text search
        ///     on the 'Title' and 'Description' fields.
        /// </remarks>
        [AllowAnonymous]
        [HttpGet(Name = nameof(ListLocations))]
        [Produces(MediaTypes.ApplicationJson)]
        [SwaggerResponse(StatusCodes.Status200OK, "Locations have been retrieved.", typeof(PagedDataDto<LocationDto>))]
        public async Task<ActionResult<PagedDataDto<LocationDto>>> ListLocations([FromQuery] ListLocationsQuery query)
        {
            return await mediator.Send(query);
        }

        /// <summary>
        ///     Create a new Location.
        /// </summary>
        [Authorize(Policy = nameof(CanModifyLocation))]
        [HttpPost(Name = nameof(CreateLocation))]
        [Produces(MediaTypes.ApplicationJson)]
        [Consumes(MediaTypes.ApplicationJson)]
        [SwaggerResponse(StatusCodes.Status201Created, "Location has been created.", typeof(LocationDto))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request model.")]
        public async Task<ActionResult<LocationDto>> CreateLocation(CreateLocationCommand command)
        {
            var location = await mediator.Send(command);

            return CreatedAtRoute(nameof(GetLocation), new { location.Moniker }, location);
        }

        /// <summary>
        ///     Update an existing location.
        /// </summary>
        [Authorize(Policy = nameof(CanModifyLocation))]
        [HttpPut("{moniker}", Name = nameof(UpdateLocation))]
        [Produces(MediaTypes.ApplicationJson)]
        [Consumes(MediaTypes.ApplicationJson)]
        [SwaggerResponse(StatusCodes.Status200OK, "Location has been updated.", typeof(LocationDto))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request model.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Location with the provided moniker does not exist.")]
        public async Task<ActionResult<LocationDto>> UpdateLocation(
            [Required, SwaggerParameter("Location moniker.")] string moniker,
            [FromBody] UpdateLocationRequestDto request)
        {
            return await mediator.Send(new UpdateLocationCommand
            {
                Moniker = moniker,
                Description = request.Description,
            });
        }
    }
}
