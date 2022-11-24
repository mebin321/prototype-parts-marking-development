namespace WebApi.Features.ProductGroup
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
    [Route("api/v1/productgroups")]
    public class ProductGroupsController : ControllerBase
    {
        private readonly IMediator mediator;

        public ProductGroupsController(IMediator mediator)
        {
            Guard.NotNull(mediator, nameof(mediator));

            this.mediator = mediator;
        }

        /// <summary>
        ///     Retrieve a specific Product Group.
        /// </summary>
        [AllowAnonymous]
        [HttpGet("{moniker}", Name = nameof(GetProductGroup))]
        [Produces(MediaTypes.ApplicationJson)]
        [SwaggerResponse(StatusCodes.Status200OK, "Product Group has been retrieved.", typeof(ProductGroupDto))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid moniker value.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Product Group with the provided moniker does not exist.")]
        public async Task<ActionResult<ProductGroupDto>> GetProductGroup(string moniker)
        {
            return await mediator.Send(new GetProductGroupQuery { Moniker = moniker });
        }

        /// <summary>
        ///     Retrieve a list of Product Groups.
        /// </summary>
        /// <remarks>
        ///     The 'search' parameter performs a full text search
        ///     on the 'Title' and 'Description' fields.
        /// </remarks>
        [AllowAnonymous]
        [HttpGet(Name = nameof(ListProductGroups))]
        [Produces(MediaTypes.ApplicationJson)]
        [SwaggerResponse(StatusCodes.Status200OK, "Product Groups have been retrieved.", typeof(PagedDataDto<ProductGroupDto>))]
        public async Task<ActionResult<PagedDataDto<ProductGroupDto>>> ListProductGroups([FromQuery] ListProductGroupsQuery query)
        {
            return await mediator.Send(query);
        }

        /// <summary>
        ///     Create a new Product Group.
        /// </summary>
        [Authorize(Policy = nameof(CanModifyProductGroup))]
        [HttpPost(Name = nameof(CreateProductGroup))]
        [Produces(MediaTypes.ApplicationJson)]
        [Consumes(MediaTypes.ApplicationJson)]
        [SwaggerResponse(StatusCodes.Status201Created, "Product Group has been created.", typeof(ProductGroupDto))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request model.")]
        public async Task<ActionResult<ProductGroupDto>> CreateProductGroup(CreateProductGroupCommand command)
        {
            var productGroup = await mediator.Send(command);

            return CreatedAtRoute(nameof(GetProductGroup), new { productGroup.Moniker }, productGroup);
        }

        /// <summary>
        ///     Update an existing Product Group.
        /// </summary>
        [Authorize(Policy = nameof(CanModifyProductGroup))]
        [HttpPut("{moniker}", Name = nameof(UpdateProductGroup))]
        [Produces(MediaTypes.ApplicationJson)]
        [Consumes(MediaTypes.ApplicationJson)]
        [SwaggerResponse(StatusCodes.Status200OK, "Product Group has been updated.", typeof(ProductGroupDto))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request model.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Product Group with the provided moniker does not exist.")]
        public async Task<ActionResult<ProductGroupDto>> UpdateProductGroup(string moniker, UpdateProductGroupRequestDto request)
        {
            return await mediator.Send(new UpdateProductGroupCommand
            {
                Moniker = moniker,
                Description = request.Description,
            });
        }
    }
}