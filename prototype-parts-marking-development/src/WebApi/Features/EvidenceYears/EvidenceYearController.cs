namespace WebApi.Features.EvidenceYears
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
    [Route("api/v1/evidence-years")]
    public class EvidenceYearController : ControllerBase
    {
        private readonly IMediator mediator;

        public EvidenceYearController(IMediator mediator)
        {
            Guard.NotNull(mediator, nameof(mediator));

            this.mediator = mediator;
        }

        /// <summary>
        ///     Retrieve a specific Evidence Year.
        /// </summary>
        [AllowAnonymous]
        [HttpGet("{year}", Name = nameof(GetEvidenceYear))]
        [Produces(MediaTypes.ApplicationJson)]
        [SwaggerResponse(StatusCodes.Status200OK, "Evidence Year has been retrieved.", typeof(EvidenceYearDto))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid year value.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Evidence Year with the provided year does not exist.")]
        public async Task<ActionResult<EvidenceYearDto>> GetEvidenceYear(int year)
        {
            return await mediator.Send(new GetEvidenceYearQuery { Year = year });
        }

        /// <summary>
        ///     Retrieve a list of Evidence Years.
        /// </summary>
        [AllowAnonymous]
        [HttpGet(Name = nameof(ListEvidenceYears))]
        [Produces(MediaTypes.ApplicationJson)]
        [SwaggerResponse(StatusCodes.Status200OK, "Evidence Years have been retrieved.", typeof(PagedDataDto<EvidenceYearDto>))]
        public async Task<ActionResult<PagedDataDto<EvidenceYearDto>>> ListEvidenceYears(
            [FromQuery] ListEvidenceYearsQuery query)
        {
            return await mediator.Send(query);
        }

        /// <summary>
        ///     Create a new Evidence Year.
        /// </summary>
        [Authorize(Policy = nameof(CanModifyEvidenceYear))]
        [HttpPost(Name = nameof(CreateEvidenceYear))]
        [Produces(MediaTypes.ApplicationJson)]
        [Consumes(MediaTypes.ApplicationJson)]
        [SwaggerResponse(StatusCodes.Status201Created, "Evidence Year has been created.", typeof(EvidenceYearDto))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request model.")]
        public async Task<ActionResult<EvidenceYearDto>> CreateEvidenceYear(CreateEvidenceYearCommand command)
        {
            var evidenceYear = await mediator.Send(command);

            return CreatedAtRoute(nameof(GetEvidenceYear), new { evidenceYear.Year }, evidenceYear);
        }
    }
}