namespace WebApi.Features.PrototypeSets.Controllers
{
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
    public class PrototypeSetController : ControllerBase
    {
        private readonly IMediator mediator;

        public PrototypeSetController(IMediator mediator)
        {
            Guard.NotNull(mediator, nameof(mediator));

            this.mediator = mediator;
        }

        /// <summary>
        ///     Retrieve a specific Prototype Set.
        /// </summary>
        [AllowAnonymous]
        [HttpGet("prototype-sets/{id}", Name = nameof(GetPrototypeSet))]
        [Produces(MediaTypes.ApplicationJson)]
        [SwaggerResponse(StatusCodes.Status200OK, "Prototype Set has been retrieved.", typeof(PrototypeSetDto))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid Id value.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Prototype Set with the provided Id does not exist.")]
        public async Task<ActionResult<PrototypeSetDto>> GetPrototypeSet(int id)
        {
           var prototypeSetDto = await mediator.Send(new GetPrototypeSetQuery { Id = id });

           return prototypeSetDto;
        }

        /// <summary>
        ///     Retrieve a filtered list of Prototype Sets .
        /// </summary>
        [AllowAnonymous]
        [HttpGet("prototype-sets", Name = nameof(ListPrototypeSets))]
        [Produces(MediaTypes.ApplicationJson)]
        [SwaggerResponse(StatusCodes.Status200OK, "Prototype Sets have been retrieved.", typeof(PagedDataDto<PrototypeSetDto>))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request model.")]
        public async Task<ActionResult<PagedDataDto<PrototypeSetDto>>> ListPrototypeSets([FromQuery] ListPrototypeSetsQuery query)
        {
            return await mediator.Send(query);
        }

        /// <summary>
        ///     Create a new Prototype Set.
        /// </summary>
        [Authorize(Policy = nameof(CanCreatePrototypeSet))]
        [HttpPost("prototype-sets", Name = nameof(CreatePrototypeSet))]
        [Produces(MediaTypes.ApplicationJson)]
        [Consumes(MediaTypes.ApplicationJson)]
        [SwaggerResponse(StatusCodes.Status201Created, "Prototype Set has been created.", typeof(PrototypeSetDto))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request model.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Some of the entities for request model does not exist.")]
        public async Task<ActionResult<PrototypeSetDto>> CreatePrototypeSet(CreatePrototypeSetCommand command)
        {
            var prototypeSet = await mediator.Send(command);

            return CreatedAtRoute(nameof(GetPrototypeSet), new { prototypeSet.Id }, prototypeSet);
        }

        /// <summary>
        ///     Delete an active Prototype Set.
        /// </summary>
        [Authorize(Policy = nameof(CanScrapPrototypeSets))]
        [HttpDelete("active-prototype-sets/{id}", Name = nameof(ScrapPrototypeSet))]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Prototype Set has been deleted.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Prototype Set with provided Id does not exist.")]
        public async Task<IActionResult> ScrapPrototypeSet(int id)
        {
            await mediator.Send(new ScrapPrototypeSetCommand { SetId = id });

            return NoContent();
        }

        /// <summary>
        ///     Reactivate a scrapped Prototype Set.
        /// </summary>
        [Authorize(Policy = nameof(CanReactivatePrototypeSets))]
        [HttpDelete("scrapped-prototype-sets/{id}", Name = nameof(ReactivatePrototypeSet))]
        [Produces(MediaTypes.ApplicationJson)]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Prototype Set has been reactivated.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Prototype Set with provided Id does not exist.")]
        public async Task<IActionResult> ReactivatePrototypeSet(int id)
        {
            await mediator.Send(new ReactivatePrototypeSetCommand { SetId = id });

            return NoContent();
        }

        /// <summary>
        ///     Retrieve details about the communication options.
        /// </summary>
        [AllowAnonymous]
        [HttpOptions("prototype-sets")]
        [Produces(MediaTypes.ApplicationJson)]
        public IActionResult PrototypesMetadata()
        {
            var mapping = HttpContext.RequestServices.GetService<ISortColumnMapping<PrototypeSet>>();

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
