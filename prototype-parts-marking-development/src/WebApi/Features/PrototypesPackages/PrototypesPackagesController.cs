namespace WebApi.Features.PrototypesPackages
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
    public class PrototypesPackagesController : ControllerBase
    {
        private readonly IMediator mediator;

        public PrototypesPackagesController(IMediator mediator)
        {
            Guard.NotNull(mediator, nameof(mediator));

            this.mediator = mediator;
        }

        /// <summary>
        ///     Retrieve a specific Prototypes Package.
        /// </summary>
        [AllowAnonymous]
        [HttpGet("prototypes-packages/{id}", Name = nameof(GetPackage))]
        [Produces(MediaTypes.ApplicationJson)]
        [SwaggerResponse(StatusCodes.Status200OK, "Prototypes Package has been retrieved.", typeof(PrototypesPackageDto))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid Id value.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Prototypes Package with the provided Id does not exist.")]
        public async Task<ActionResult<PrototypesPackageDto>> GetPackage(int id)
        {
            return await mediator.Send(new GetPrototypesPackageQuery { Id = id });
        }

        /// <summary>
        ///     Retrieve a filtered list of Prototypes Packages.
        /// </summary>
        [AllowAnonymous]
        [HttpGet("prototypes-packages", Name = nameof(ListPackages))]
        [Produces(MediaTypes.ApplicationJson)]
        [SwaggerResponse(StatusCodes.Status200OK, "Prototypes Packages have been retrieved.", typeof(PagedDataDto<PrototypesPackageDto>))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request model.")]
        public async Task<ActionResult<PagedDataDto<PrototypesPackageDto>>> ListPackages([FromQuery] ListPrototypesPackagesQuery query)
        {
            return await mediator.Send(query);
        }

        /// <summary>
        ///     Create a new Prototypes Package.
        /// </summary>
        [Authorize(Policy = nameof(CanCreatePrototypePackages))]
        [HttpPost("prototypes-packages", Name = nameof(CreatePackage))]
        [Produces(MediaTypes.ApplicationJson)]
        [Consumes(MediaTypes.ApplicationJson)]
        [SwaggerResponse(StatusCodes.Status201Created, "Prototypes Package has been created.", typeof(PrototypesPackageDto))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request model.")]
        [SwaggerResponse(StatusCodes.Status409Conflict, " Concurrency conflicts.")]
        [SwaggerResponse(StatusCodes.Status412PreconditionFailed, "Missing If-Match Header or version of resource is out of date.")]
        public async Task<ActionResult<PrototypesPackageDto>> CreatePackage(CreatePrototypesPackageCommand command)
        {
            var package = await mediator.Send(command);

            return CreatedAtRoute(nameof(GetPackage), new { package.Id }, package);
        }

        /// <summary>
        ///     Update an existing Prototypes Package.
        /// </summary>
        [Authorize(Policy = nameof(CanModifyPrototypePackages))]
        [HttpPut("prototypes-packages/{id}", Name = nameof(UpdatePackage))]
        [Produces(MediaTypes.ApplicationJson)]
        [Consumes(MediaTypes.ApplicationJson)]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Prototypes Package has been updated.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Prototypes Package with the provided Id does not exist.")]
        [SwaggerResponse(StatusCodes.Status412PreconditionFailed, "Missing If-Match Header or version of resource is out of date.")]
        public async Task<IActionResult> UpdatePackage(int id, UpdatePrototypesPackageDto dto)
        {
            await mediator.Send(new UpdatePrototypesPackageCommand
            {
                Id = id,
                OwnerId = dto.Owner,
                Commment = dto.Comment,
                ActualCount = dto.ActualCount,
            });

            return NoContent();
        }

        /// <summary>
        ///     Scrap an active Prototypes Package.
        /// </summary>
        [Authorize(Policy = nameof(CanScrapPrototypesPackages))]
        [HttpDelete("active-prototypes-packages/{id}", Name = nameof(ScrapPrototypesPackage))]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Prototypes Package has been scrapped.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Prototypes Package with provided Id does not exist.")]
        public async Task<IActionResult> ScrapPrototypesPackage(int id)
        {
            await mediator.Send(new ScrapPrototypesPackageCommand { Id = id });

            return NoContent();
        }

        /// <summary>
        ///     Reactivate a scrapped Prototypes Package.
        /// </summary>
        [Authorize(Policy = nameof(CanReactivatePrototypesPackages))]
        [HttpDelete("scrapped-prototypes-packages/{id}", Name = nameof(ReactivatePrototypesPackage))]
        [Produces(MediaTypes.ApplicationJson)]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Prototypes Package has been reactivated.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Prototypes Package with provided Id does not exist.")]
        public async Task<IActionResult> ReactivatePrototypesPackage(int id)
        {
            await mediator.Send(new ReactivatePrototypesPackageCommand { Id = id });

            return NoContent();
        }

        /// <summary>
        ///     Retrieve details about the communication options.
        /// </summary>
        [AllowAnonymous]
        [HttpOptions("prototypes-packages")]
        [Produces(MediaTypes.ApplicationJson)]
        public IActionResult PrototypesMetadata()
        {
            var mapping = HttpContext.RequestServices.GetService<ISortColumnMapping<PrototypesPackage>>();

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