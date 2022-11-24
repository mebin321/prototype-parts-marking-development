namespace WebApi.Features.PrototypeVariants
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
    [Route("api/v1")]
    public class PrototypeVariantsController : ControllerBase
    {
        private readonly IMediator mediator;

        public PrototypeVariantsController(IMediator mediator)
        {
            Guard.NotNull(mediator, nameof(mediator));

            this.mediator = mediator;
        }

        /// <summary>
        ///     Retrieve a specific Prototype Variant.
        /// </summary>
        [AllowAnonymous]
        [HttpGet("prototype-sets/{setId}/prototypes/{prototypeId}/variants/{variantVersion}", Name = nameof(GetPrototypeVariant))]
        [Produces(MediaTypes.ApplicationJson)]
        [SwaggerResponse(StatusCodes.Status200OK, "Prototype variant has been retrieved.", typeof(PrototypeVariantDto))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid SetId, PrototypeId or VariantWVersion value.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Prototype Set Id, Prototype Id or Prototype Variant version does not exist.")]
        public async Task<ActionResult<PrototypeVariantDto>> GetPrototypeVariant(int setId, int prototypeId, int variantVersion)
        {
            return await mediator.Send(new GetPrototypeVariantQuery { SetId = setId, PrototypeId = prototypeId, VariantVersion = variantVersion });
        }

        /// <summary>
        ///     Retrieve a list of Prototype Variants.
        /// </summary>
        [AllowAnonymous]
        [HttpGet("prototype-sets/{setId}/prototypes/{prototypeId}/variants", Name = nameof(ListPrototypeVariants))]
        [Produces(MediaTypes.ApplicationJson)]
        [SwaggerResponse(StatusCodes.Status200OK, "Prototype variants have been retrieved.", typeof(PagedDataDto<PrototypeVariantDto>))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid SetId or PrototypeId value.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Prototype Set or Prototype with provided Id does not exist.")]
        public async Task<ActionResult<PagedDataDto<PrototypeVariantDto>>> ListPrototypeVariants(
            int setId,
            int prototypeId,
            [FromQuery] ListPrototypeVariantsRequestDto request)
        {
            return await mediator.Send(new ListPrototypeVariantsQuery
            {
                Page = request.Page,
                PageSize = request.PageSize,
                SetId = setId,
                PrototypeId = prototypeId,
            });
        }

        /// <summary>
        ///      Retrieve a filtered list of Prototype Variants.
        /// </summary>
        [AllowAnonymous]
        [HttpGet("variants", Name = nameof(FilteredListVariants))]
        [Produces(MediaTypes.ApplicationJson)]
        [SwaggerResponse(StatusCodes.Status200OK, "Variants have been retrieved.", typeof(PagedDataDto<EnrichPrototypeVariantDto>))]
        public async Task<ActionResult<PagedDataDto<EnrichPrototypeVariantDto>>> FilteredListVariants([FromQuery] FilteredListPrototypeVariantsQuery query)
        {
            return await mediator.Send(query);
        }

        /// <summary>
        ///     Create a new Prototype Variant.
        /// </summary>
        [Authorize(Policy = nameof(CanModifyPrototypeVariants))]
        [HttpPost("prototype-sets/{setId}/prototypes/{prototypeId}/variants", Name = nameof(CreatePrototypeVariant))]
        [Produces(MediaTypes.ApplicationJson)]
        [Consumes(MediaTypes.ApplicationJson)]
        [SwaggerResponse(StatusCodes.Status201Created, "Prototype Variant has been created.", typeof(PrototypeVariantDto))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request model.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Prototype Set or Prototype with provided Id does not exist.")]
        [SwaggerResponse(StatusCodes.Status409Conflict, " Concurrency conflicts.")]
        [SwaggerResponse(StatusCodes.Status412PreconditionFailed, "Missing If-Match Header or version of resource is out of date.")]
        public async Task<ActionResult<PrototypeVariantDto>> CreatePrototypeVariant(int setId, int prototypeId, CreatePrototypeVariantRequestDto request)
        {
            var prototypeVariant = await mediator.Send(new CreatePrototypeVariantCommand
            {
                SetId = setId,
                PrototypeId = prototypeId,
                Comment = request.Comment,
            });

            return CreatedAtRoute(
                nameof(GetPrototypeVariant),
                new { setId, prototypeId, variantVersion = prototypeVariant.Version },
                prototypeVariant);
        }
    }
}