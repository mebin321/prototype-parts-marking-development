namespace WebApi.Features.Parts.Controllers
{
    using System.Threading.Tasks;
    using MediatR;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Swashbuckle.AspNetCore.Annotations;
    using Utilities;
    using WebApi.Features.Authorization.Policies;
    using WebApi.Features.Parts.Models;
    using WebApi.Features.Parts.Requests;

    [Authorize]
    [ApiController]
    [Route("api/v1/productgroups/{moniker}/parts")]
    public class ProductGroupPartsController : ControllerBase
    {
        private readonly IMediator mediator;

        public ProductGroupPartsController(IMediator mediator)
        {
            Guard.NotNull(mediator, nameof(mediator));

            this.mediator = mediator;
        }

        /// <summary>
        ///    Retrieve a filtered list of Parts assigned to a specific Product Group.
        /// </summary>
        [AllowAnonymous]
        [HttpGet]
        [Authorize(Policy = nameof(CanReadEntityRelations))]
        [SwaggerOperation(Tags = new[] { "ProductGroups" })]
        [SwaggerResponse(StatusCodes.Status200OK, "Parts assigned to a specific Product Group have been retrieved.", typeof(PartDto[][]))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid moniker value.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Product Group with the provided moniker does not exist.")]
        public async Task<ActionResult<PartDto[]>> ListRelatedParts([FromQuery] ListRelatedPartsQuery query)
        {
            return await mediator.Send(query);
        }

        /// <summary>
        ///     Update Parts assigned to a specific Product Group.
        /// </summary>
        [Authorize(Policy = nameof(CanModifyEntityRelation))]
        [HttpPut]
        [Consumes(MediaTypes.ApplicationJson)]
        [SwaggerOperation(Tags = new[] { "ProductGroups" })]
        [SwaggerResponse(StatusCodes.Status200OK, "Parts assigned to a specific Product Group have been updated.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request model.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Product Group with the provided moniker does not exist.")]
        public async Task<ActionResult> UpdateRelatedParts(string moniker, string[] productGroupMonikers)
        {
            await mediator.Send(new UpdateRelatedPartsCommand
            {
                Moniker = moniker,
                PartMonikers = productGroupMonikers,
            });

            return NoContent();
        }
    }
}