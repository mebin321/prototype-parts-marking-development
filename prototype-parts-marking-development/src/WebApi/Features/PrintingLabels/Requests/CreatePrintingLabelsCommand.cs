namespace WebApi.Features.PrintingLabels.Requests
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Authentication.Services;
    using Common;
    using Data;
    using FluentValidation;
    using MediatR;
    using Microsoft.EntityFrameworkCore;
    using Models;
    using Utilities;

    public class CreatePrintingLabelsCommand : IRequest<List<PrintingLabelDto>>
    {
        [Required]
        public List<CreatePrintingLabelRequestDto> PrintingLabels { get; set; }

        public class Handler : IRequestHandler<CreatePrintingLabelsCommand, List<PrintingLabelDto>>
        {
            private readonly IDbContextFactory<PrototypePartsDbContext> dbContextFactory;
            private readonly ICurrentUserAccessor currentUserAccessor;
            private readonly IDateTime dateTime;

            public Handler(
                IDbContextFactory<PrototypePartsDbContext> dbContextFactory,
                ICurrentUserAccessor currentUserAccessor,
                IDateTime dateTime)
            {
                Guard.NotNull(dbContextFactory, nameof(dbContextFactory));
                Guard.NotNull(currentUserAccessor, nameof(currentUserAccessor));
                Guard.NotNull(dateTime, nameof(dateTime));

                this.dbContextFactory = dbContextFactory;
                this.currentUserAccessor = currentUserAccessor;
                this.dateTime = dateTime;
            }

            public async Task<List<PrintingLabelDto>> Handle(
                CreatePrintingLabelsCommand request,
                CancellationToken cancellationToken)
            {
                await using var dbContext = dbContextFactory.CreateDbContext();

                var currentUser = currentUserAccessor.GetCurrentUser();
                var timestamp = dateTime.Now;
                var createdPrintingLabels = new List<PrintingLabel>();

                foreach (var requestedPrintingLabel in request.PrintingLabels)
                {
                    var printingLabel = new PrintingLabel
                    {
                        OwnerId = currentUser,
                        Customer = requestedPrintingLabel.Customer,
                        ProductGroup = requestedPrintingLabel.ProductGroup,
                        Outlet = requestedPrintingLabel.Outlet,
                        Location = requestedPrintingLabel.Location,
                        ProjectNumber = requestedPrintingLabel.ProjectNumber,
                        GateLevel = requestedPrintingLabel.GateLevel,
                        MaterialNumber = requestedPrintingLabel.MaterialNumber,
                        RevisionCode = requestedPrintingLabel.RevisionCode,
                        Description = requestedPrintingLabel.Description,
                        PartType = requestedPrintingLabel.PartType,
                        PartCode = requestedPrintingLabel.PartCode,
                        CreatedAt = timestamp,
                    };

                    createdPrintingLabels.Add(printingLabel);
                }

                dbContext.PrintingLabels.AddRange(createdPrintingLabels);
                await dbContext.SaveChangesAsync();

                return createdPrintingLabels
                    .Select(p => PrintingLabelDto.From(p))
                    .ToList();
            }
        }

        public class Validator : AbstractValidator<CreatePrintingLabelsCommand>
        {
            public Validator()
            {
                RuleFor(r => r.PrintingLabels).NotEmpty();
                RuleForEach(r => r.PrintingLabels).NotEmpty();
            }
        }
    }
}