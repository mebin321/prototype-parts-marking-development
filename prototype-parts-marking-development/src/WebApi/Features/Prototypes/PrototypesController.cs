namespace WebApi.Features.Prototypes
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Common.Paging;
    using MediatR;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.DependencyInjection;
    using Models;
    using Requests;
    using Swashbuckle.AspNetCore.Annotations;
    using Utilities;
    using WebApi.Common.Sorting;
    using WebApi.Data;
    using WebApi.Features.Authorization.Policies;

    [Authorize]
    [ApiController]
    [Route("api/v1")]
    public class PrototypesController : ControllerBase
    {
        private readonly IMediator mediator;

        public PrototypesController(IMediator mediator)
        {
            Guard.NotNull(mediator, nameof(mediator));

            this.mediator = mediator;
        }

        /// <summary>
        ///     Retrieve a specific Prototype.
        /// </summary>
        [AllowAnonymous]
        [HttpGet("prototype-sets/{setId}/prototypes/{prototypeId}", Name = nameof(GetPrototype))]
        [Produces(MediaTypes.ApplicationJson)]
        [SwaggerResponse(StatusCodes.Status200OK, "Prototype has been retrieved.", typeof(PrototypeDto))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid SetId or PrototypeId value.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Prototype with the provided Id does not exist in PrototypeSet with provided Id.")]
        public async Task<ActionResult<PrototypeDto>> GetPrototype(int setId, int prototypeId)
        {
            return await mediator.Send(new GetPrototypeQuery { SetId = setId, PrototypeId = prototypeId });
        }

        /// <summary>
        ///     Retrieve a list of prototypes.
        /// </summary>
        [AllowAnonymous]
        [HttpGet("prototype-sets/{setId}/prototypes", Name = nameof(ListPrototypes))]
        [Produces(MediaTypes.ApplicationJson)]
        [SwaggerResponse(StatusCodes.Status200OK, "Prototypes have been retrieved.", typeof(PagedDataDto<PrototypeDto>))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid Type of prototype.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "PrototypeSet with provided Id does not exist.")]
        public async Task<ActionResult<PagedDataDto<PrototypeDto>>> ListPrototypes([FromQuery] ListPrototypesQuery query)
        {
            return await mediator.Send(query);
        }

        /// <summary>
        ///     Retrieve a list of active prototypes.
        /// </summary>
        [AllowAnonymous]
        [HttpGet("prototype-sets/{setId}/active-prototypes", Name = nameof(ListActivePrototypes))]
        [Produces(MediaTypes.ApplicationJson)]
        [SwaggerResponse(StatusCodes.Status200OK, "Prototypes have been retrieved.", typeof(PagedDataDto<PrototypeDto>))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid Type of prototype.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "PrototypeSet with provided Id does not exist.")]
        public async Task<ActionResult<PagedDataDto<PrototypeDto>>> ListActivePrototypes(int setId, [FromQuery] ListPrototypesRequestDto request)
        {
            return await mediator.Send(
                new ListPrototypesQuery()
                {
                    SetId = setId,
                    Page = request.Page,
                    PageSize = request.PageSize,
                    Index = request.Index,
                    Type = request.Type,
                    IsActive = true,
                    SortBy = request.SortBy,
                    SortDirection = request.SortDirection,
                });
        }

        /// <summary>
        ///     Retrieve a list of scrapped prototypes.
        /// </summary>
        [AllowAnonymous]
        [HttpGet("prototype-sets/{setId}/scrapped-prototypes", Name = nameof(ListScrappedPrototypes))]
        [Produces(MediaTypes.ApplicationJson)]
        [SwaggerResponse(StatusCodes.Status200OK, "Prototypes have been retrieved.", typeof(PagedDataDto<PrototypeDto>))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid Type of prototype.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "PrototypeSet with provided Id does not exist.")]
        public async Task<ActionResult<PagedDataDto<PrototypeDto>>> ListScrappedPrototypes(int setId, [FromQuery] ListPrototypesRequestDto request)
        {
            return await mediator.Send(
                new ListPrototypesQuery()
                {
                    SetId = setId,
                    Page = request.Page,
                    PageSize = request.PageSize,
                    Index = request.Index,
                    Type = request.Type,
                    IsActive = false,
                    SortBy = request.SortBy,
                    SortDirection = request.SortDirection,
                });
        }

        /// <summary>
        ///     Retrieve a filtered list of Prototypes.
        /// </summary>
        [AllowAnonymous]
        [HttpGet("prototypes", Name = nameof(FilteredListPrototypes))]
        [Produces(MediaTypes.ApplicationJson)]
        [SwaggerResponse(StatusCodes.Status200OK, "Prototypes have been retrieved.", typeof(PagedDataDto<EnrichedPrototypeDto>))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request model.")]
        public async Task<ActionResult<PagedDataDto<EnrichedPrototypeDto>>> FilteredListPrototypes([FromQuery] FilteredListPrototypesQuery query)
        {
            return await mediator.Send(query);
        }

        /// <summary>
        ///     Create new Prototypes of type Original.
        /// </summary>
        [Authorize(Policy = nameof(CanCreatePrototypes))]
        [HttpPost("prototype-sets/{setId}/prototypes/", Name = nameof(CreatePrototype))]
        [Produces(MediaTypes.ApplicationJson)]
        [Consumes(MediaTypes.ApplicationJson)]
        [SwaggerResponse(StatusCodes.Status201Created, "Prototypes of type Original have been created.", typeof(List<CreatedPrototypeDto>))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request model.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "PrototypeSet with provided Id does not exist.")]
        [SwaggerResponse(StatusCodes.Status409Conflict, " Concurrency conflicts.")]
        [SwaggerResponse(StatusCodes.Status412PreconditionFailed, "Missing If-Match Header or version of resource is out of date.")]
        public async Task<ActionResult<List<CreatedPrototypeDto>>> CreatePrototype(int setId, List<CreatePrototypesRequestDto> request)
        {
            return await mediator.Send(new CreatePrototypesCommand() { SetId = setId, Prototypes = request });
        }

        /// <summary>
        ///     Create new Prototypes of type Component.
        /// </summary>
        [Authorize(Policy = nameof(CanCreatePrototypes))]
        [HttpPost("prototype-sets/{setId}/components/", Name = nameof(CreateComponents))]
        [Produces(MediaTypes.ApplicationJson)]
        [Consumes(MediaTypes.ApplicationJson)]
        [SwaggerResponse(StatusCodes.Status201Created, "Prototypes of type Component has been created.", typeof(List<CreatedPrototypeDto>))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request model.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "PrototypeSet with provided Id does not exist.")]
        [SwaggerResponse(StatusCodes.Status409Conflict, " Concurrency conflicts.")]
        [SwaggerResponse(StatusCodes.Status412PreconditionFailed, "Missing If-Match Header or version of resource is out of date.")]
        public async Task<ActionResult<List<CreatedPrototypeDto>>> CreateComponents(int setId, List<CreatePrototypesRequestDto> request)
        {
            return await mediator.Send(new CreateComponentsCommand() { SetId = setId, Prototypes = request });
        }

        /// <summary>
        ///     Update an existing Prototype.
        /// </summary>
        [Authorize(Policy = nameof(CanModifyPrototypes))]
        [HttpPut("prototype-sets/{setId}/prototypes/{prototypeId}", Name = nameof(UpdatePrototype))]
        [Produces(MediaTypes.ApplicationJson)]
        [Consumes(MediaTypes.ApplicationJson)]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Prototype has been updated.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Prototype with the provided Id does not exist in PrototypeSet with provided Id.")]
        public async Task<IActionResult> UpdatePrototype(int setId, int prototypeId, UpdatePrototypeDto dto)
        {
            await mediator.Send(new UpdatePrototypeCommand
            {
                SetId = setId,
                PrototypeId = prototypeId,
                OwnerId = dto.Owner,
                Comment = dto.Comment,
                MaterialNumber = dto.MaterialNumber,
                RevisionCode = dto.RevisionCode,
            });

            return NoContent();
        }

        /// <summary>
        ///     Scrap an active Prototype.
        /// </summary>
        [Authorize(Policy = nameof(CanScrapPrototypes))]
        [HttpDelete("prototype-sets/{setId}/active-prototypes/{prototypeId}", Name = nameof(ScrapPrototype))]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Prototype has been scrapped.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "PrototypeSet or Prototype with provided Id does not exist.")]
        public async Task<IActionResult> ScrapPrototype(int setId, int prototypeId)
        {
            await mediator.Send(new ScrapPrototypeCommand { SetId = setId, PrototypeId = prototypeId });

            return NoContent();
        }

        /// <summary>
        ///     Reactivate a scrapped Prototype.
        /// </summary>
        [Authorize(Policy = nameof(CanReactivatePrototypes))]
        [HttpDelete("prototype-sets/{setId}/scrapped-prototypes/{prototypeId}", Name = nameof(ReactivatePrototype))]
        [Produces(MediaTypes.ApplicationJson)]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Prototype has been reactivated.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "PrototypeSet or Prototype with provided Id does not exist.")]
        public async Task<IActionResult> ReactivatePrototype(int setId, int prototypeId)
        {
            await mediator.Send(new ReactivatePrototypeCommand { SetId = setId, PrototypeId = prototypeId });

            return NoContent();
        }

        /// <summary>
        ///     Retrieve details about the communication options.
        /// </summary>
        [AllowAnonymous]
        [HttpOptions("prototypes")]
        [Produces(MediaTypes.ApplicationJson)]
        public IActionResult PrototypesMetadata()
        {
            var mapping = HttpContext.RequestServices.GetService<ISortColumnMapping<Prototype>>();

            var metadata = new
            {
                SortDirections = new[]
                {
                    SortDirection.Ascending,
                    SortDirection.Descending,
                },
                SortableColumns = mapping?.MappedValues,
            };

            return Ok(metadata);
        }
    }
}