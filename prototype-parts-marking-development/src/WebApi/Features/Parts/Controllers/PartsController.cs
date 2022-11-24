namespace WebApi.Features.Parts.Controllers
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
    [Route("api/v1/parts")]
    public class PartsController : ControllerBase
    {
        private readonly IMediator mediator;

        public PartsController(IMediator mediator)
        {
            Guard.NotNull(mediator, nameof(mediator));

            this.mediator = mediator;
        }

        /// <summary>
        ///     Retrieve a specific Part.
        /// </summary>
        [AllowAnonymous]
        [HttpGet("{moniker}", Name = nameof(GetPart))]
        [Produces(MediaTypes.ApplicationJson)]
        [SwaggerResponse(StatusCodes.Status200OK, "Part has been retrieved.", typeof(PartDto))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid moniker value.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Part with the provided moniker does not exist.")]
        public async Task<ActionResult<PartDto>> GetPart(
            [Required, SwaggerParameter("Part moniker.")] string moniker)
        {
            return await mediator.Send(new GetPartQuery { Moniker = moniker });
        }

        /// <summary>
        ///     Retrieve a list of Parts.
        /// </summary>
        /// <remarks>
        ///     The 'search' parameter performs a full text search
        ///     on the 'Title' and 'Description' fields.
        /// </remarks>
        [AllowAnonymous]
        [HttpGet(Name = nameof(ListParts))]
        [Produces(MediaTypes.ApplicationJson)]
        [SwaggerResponse(StatusCodes.Status200OK, "Parts have been retrieved.", typeof(PagedDataDto<PartDto>))]
        public async Task<ActionResult<PagedDataDto<PartDto>>> ListParts([FromQuery] ListPartsQuery query)
        {
            return await mediator.Send(query);
        }

        /// <summary>
        ///     Create a new Part.
        /// </summary>
        [Authorize(Policy = nameof(CanModifyPart))]
        [HttpPost(Name = nameof(CreatePart))]
        [Produces(MediaTypes.ApplicationJson)]
        [Consumes(MediaTypes.ApplicationJson)]
        [SwaggerResponse(StatusCodes.Status201Created, "Part has been created.", typeof(PartDto))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request model.")]
        public async Task<ActionResult<PartDto>> CreatePart(CreatePartCommand command)
        {
            var part = await mediator.Send(command);

            return CreatedAtRoute(nameof(GetPart), new { part.Moniker }, part);
        }

        /// <summary>
        ///     Update an existing Part.
        /// </summary>
        [Authorize(Policy = nameof(CanModifyPart))]
        [HttpPut("{moniker}", Name = nameof(UpdatePart))]
        [Produces(MediaTypes.ApplicationJson)]
        [Consumes(MediaTypes.ApplicationJson)]
        [SwaggerResponse(StatusCodes.Status200OK, "Part has been updated.", typeof(PartDto))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request model.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Part with the provided moniker does not exist.")]
        public async Task<ActionResult<PartDto>> UpdatePart(
            [Required, SwaggerParameter("Part moniker.")] string moniker,
            [FromBody] UpdatePartRequestDto request)
        {
            return await mediator.Send(new UpdatePartCommand
            {
                Moniker = moniker,
                Description = request.Description,
            });
        }

        /// <summary>
        ///    Retrieve a filtered list of Components assigned to a specific Part.
        /// </summary>
        [Authorize(Policy = nameof(CanReadEntityRelations))]
        [HttpGet("{moniker}/component-parts")]
        [Produces(MediaTypes.ApplicationJson)]
        [SwaggerResponse(StatusCodes.Status200OK, "Components assigned to a specific Part has been retrieved.", typeof(PartDto[]))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid moniker value.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Part with the provided moniker does not exist.")]
        public async Task<ActionResult<PartDto[]>> ListComponentPart([FromQuery] ListComponentPartsQuery query)
        {
            return await mediator.Send(query);
        }

        /// <summary>
        ///     Update Components assigned to a specific Part.
        /// </summary>
        [Authorize(Policy = nameof(CanModifyEntityRelation))]
        [HttpPut("{moniker}/component-parts")]
        [Consumes(MediaTypes.ApplicationJson)]
        [SwaggerResponse(StatusCodes.Status200OK, "Components assigned to a specific Part have been updated.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request model.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Part with the provided moniker does not exist.")]
        public async Task<ActionResult> UpdateComponentParts(string moniker, string[] componentPartMonikers)
        {
            await mediator.Send(new UpdateComponentPartsCommand
            {
                Moniker = moniker,
                ComponentPartMonikers = componentPartMonikers,
            });

            return NoContent();
        }
    }
}