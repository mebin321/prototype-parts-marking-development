namespace WebApi.Features.GlobalProjects
{
    using System.Threading.Tasks;
    using Common.Paging;
    using Locations.Models;
    using MediatR;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Swashbuckle.AspNetCore.Annotations;
    using Utilities;
    using WebApi.Features.GlobalProjects.Models;
    using WebApi.Features.GlobalProjects.Requests;

    [Authorize]
    [ApiController]
    [Route("api/v1/")]
    public class GlobalProjectsController : ControllerBase
    {
        private readonly IMediator mediator;

        public GlobalProjectsController(IMediator mediator)
        {
            Guard.NotNull(mediator, nameof(mediator));

            this.mediator = mediator;
        }

        /// <summary>
        ///     Retrieve a list of customers.
        /// </summary>
        [AllowAnonymous]
        [HttpGet("customers")]
        [Produces(MediaTypes.ApplicationJson)]
        [SwaggerResponse(StatusCodes.Status200OK, "Customers have been retrieved.", typeof(string[]))]
        public async Task<ActionResult<string[]>> GetCustomers()
        {
            return await mediator.Send(new GetCustomersQuery());
        }

        /// <summary>
        ///     Retrieve a list of Global Projects.
        /// </summary>
        /// <remarks>
        ///     Returns at most the first 100 results.
        /// </remarks>
        [AllowAnonymous]
        [HttpGet("global-projects")]
        [Produces(MediaTypes.ApplicationJson)]
        [SwaggerResponse(StatusCodes.Status200OK, "Global Projects have been retrieved.", typeof(string[]))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request model.")]
        public async Task<ActionResult<GlobalProjectDto[]>> GetProjects([FromQuery] GetGlobalProjectsQuery query)
        {
            return await mediator.Send(query);
        }
    }
}
