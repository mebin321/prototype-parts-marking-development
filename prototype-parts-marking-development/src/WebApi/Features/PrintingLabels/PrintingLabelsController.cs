namespace WebApi.Features.PrintingLabels
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Authorization.Policies;
    using Common.Paging;
    using MediatR;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using Requests;
    using Swashbuckle.AspNetCore.Annotations;
    using Utilities;

    [Authorize]
    [ApiController]
    [Route("api/v1/printing-labels")]
    public class PrintingLabelsController : ControllerBase
    {
        private readonly IMediator mediator;

        public PrintingLabelsController(IMediator mediator)
        {
            Guard.NotNull(mediator, nameof(mediator));

            this.mediator = mediator;
        }

        /// <summary>
        ///     Retrieve a list of printing labels.
        /// </summary>
        [Authorize(Policy = nameof(CanModifyPrintingLabels))]
        [HttpGet(Name = nameof(ListPrintingLabels))]
        [Produces(MediaTypes.ApplicationJson)]
        [SwaggerResponse(StatusCodes.Status200OK, "Printing labels have been retrieved.", typeof(PagedDataDto<PrintingLabelDto>))]
        public async Task<ActionResult<PagedDataDto<PrintingLabelDto>>> ListPrintingLabels([FromQuery] ListPrintingLabelsQuery query)
        {
            return await mediator.Send(query);
        }

        /// <summary>
        ///     Create a new printing labels.
        /// </summary>
        [Authorize(Policy = nameof(CanModifyPrintingLabels))]
        [HttpPost(Name = nameof(CreatePrintingLabel))]
        [Produces(MediaTypes.ApplicationJson)]
        [Consumes(MediaTypes.ApplicationJson)]
        [SwaggerResponse(StatusCodes.Status201Created, "Printing labels has been created.", typeof(PrintingLabelDto))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request model.")]
        public async Task<ActionResult<List<PrintingLabelDto>>> CreatePrintingLabel(List<CreatePrintingLabelRequestDto> printingLabels)
        {
            return await mediator.Send(new CreatePrintingLabelsCommand() { PrintingLabels = printingLabels });
        }

        /// <summary>
        ///     Delete a printing label.
        /// </summary>
        [Authorize(Policy = nameof(CanModifyPrintingLabels))]
        [HttpDelete("{labelId}", Name = nameof(DeletePrintingLabel))]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Printing label has been deleted.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Printing label with provided Id does not exist.")]
        public async Task<IActionResult> DeletePrintingLabel(int labelId)
        {
            await mediator.Send(new DeletePrintingLabelCommand { LabelId = labelId });

            return NoContent();
        }
    }
}