namespace WebApi.Features.ProductGroup
{
    using System.Threading.Tasks;
    using MediatR;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Swashbuckle.AspNetCore.Annotations;
    using Utilities;
    using WebApi.Features.Authorization.Policies;
    using WebApi.Features.ProductGroup.Models;
    using WebApi.Features.ProductGroup.Requests;

    [Authorize]
    [ApiController]
    [Route("api/v1/outlets/{moniker}/productgroups")]
    public class OutletProductGroupsController : ControllerBase
    {
        private readonly IMediator mediator;

        public OutletProductGroupsController(IMediator mediator)
        {
            Guard.NotNull(mediator, nameof(mediator));

            this.mediator = mediator;
        }

        /// <summary>
        ///    Retrieve a filtered list of Product Groups assigned to a specific Outlet.
        /// </summary>
        [Authorize(Policy = nameof(CanReadEntityRelations))]
        [HttpGet]
        [Produces(MediaTypes.ApplicationJson)]
        [SwaggerOperation(Tags = new[] { "Outlets" })]
        [SwaggerResponse(StatusCodes.Status200OK, "Product Groups assigned to a specific Outlet have been retrieved.", typeof(ProductGroupDto[]))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid moniker value.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Outlet with the provided moniker does not exist.")]
        public async Task<ActionResult<ProductGroupDto[]>> ListRelatedProductGroups([FromQuery] ListRelatedProductGroupsQuery query)
        {
            return await mediator.Send(query);
        }

        /// <summary>
        ///     Update Product Groups assigned to a specific Outlet.
        /// </summary>
        [Authorize(Policy = nameof(CanModifyEntityRelation))]
        [HttpPut]
        [Consumes(MediaTypes.ApplicationJson)]
        [SwaggerOperation(Tags = new[] { "Outlets" })]
        [SwaggerResponse(StatusCodes.Status200OK, "Product Groups assigned to a specific Outlet have been updated.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request model.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Outlet with the provided moniker does not exist.")]
        public async Task<ActionResult> UpdateRelatedProductGroups(string moniker, string[] productGroupMonikers)
        {
            await mediator.Send(new UpdateRelatedProductGroupsCommand
            {
                Moniker = moniker,
                ProductGroupMonikers = productGroupMonikers,
            });

            return NoContent();
        }
    }
}