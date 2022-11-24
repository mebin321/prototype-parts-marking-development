namespace WebApi.Features.Parts.Requests
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using FluentValidation;
    using MediatR;
    using Microsoft.EntityFrameworkCore;
    using Utilities;
    using WebApi.Data;

    public class UpdateComponentPartsCommand : IRequest
    {
        public string Moniker { get; set; }

        public string[] ComponentPartMonikers { get; set; }

        public class Handler : AsyncRequestHandler<UpdateComponentPartsCommand>
        {
            private readonly IDbContextFactory<PrototypePartsDbContext> dbContextFactory;
            private readonly IProblemDetailsFactory problemDetailsFactory;

            public Handler(IDbContextFactory<PrototypePartsDbContext> dbContextFactory, IProblemDetailsFactory problemDetailsFactory)
            {
                Guard.NotNull(dbContextFactory, nameof(dbContextFactory));
                Guard.NotNull(problemDetailsFactory, nameof(problemDetailsFactory));

                this.dbContextFactory = dbContextFactory;
                this.problemDetailsFactory = problemDetailsFactory;
            }

            protected override async Task Handle(UpdateComponentPartsCommand request, CancellationToken cancellationToken)
            {
                await using var dbContext = dbContextFactory.CreateDbContext();

                var part = await dbContext.Parts
                    .Include(e => e.PartToComponentRelations)
                    .FirstOrDefaultAsync(e => e.Moniker == request.Moniker, CancellationToken.None)
                    .ThrowIfNullAsync(problemDetailsFactory.EntityNotFound(nameof(Part), request.Moniker));

                var monikers = request.ComponentPartMonikers.Distinct().ToList();
                if (monikers.Count == 0)
                {
                    part.PartToComponentRelations.Clear();
                }
                else
                {
                    var componentParts = await dbContext.Parts
                        .Where(e => monikers.Contains(e.Moniker))
                        .ToListAsync(CancellationToken.None);

                    if (monikers.Count != componentParts.Count)
                    {
                        throw new BadRequestException(problemDetailsFactory.BadRequest(
                            "Invalid Part.",
                            "Request contains one or more invalid Parts."));
                    }

                    part.PartToComponentRelations.Clear();
                    part.PartToComponentRelations.AddRange(componentParts.Select(p => new PartToComponentPartRelation
                    {
                        PartId = part.Id,
                        ComponentPartId = p.Id,
                    }));
                }

                await dbContext.SaveChangesAsync(CancellationToken.None);
            }
        }

        public class Validator : AbstractValidator<UpdateComponentPartsCommand>
        {
            public Validator()
            {
                RuleFor(r => r.Moniker).NotEmpty();
                RuleForEach(r => r.ComponentPartMonikers).NotEmpty();
                RuleFor(r => r)
                    .Must(r => !r.ComponentPartMonikers.Contains(r.Moniker))
                    .WithMessage("Part may not have a component relation with itself.");
            }
        }
    }
}