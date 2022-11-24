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

    public class UpdateRelatedPartsCommand : IRequest
    {
        public string Moniker { get; set; }

        public string[] PartMonikers { get; set; }

        public class Handler : AsyncRequestHandler<UpdateRelatedPartsCommand>
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

            protected override async Task Handle(UpdateRelatedPartsCommand request, CancellationToken cancellationToken)
            {
                await using var dbContext = dbContextFactory.CreateDbContext();

                var productGroup = await dbContext.ProductGroups
                    .Include(e => e.ProductGroupToPartRelations)
                    .FirstOrDefaultAsync(e => e.Moniker == request.Moniker, CancellationToken.None)
                    .ThrowIfNullAsync(problemDetailsFactory.EntityNotFound(nameof(ProductGroup), request.Moniker));

                var monikers = request.PartMonikers.Distinct().ToList();

                if (monikers.Count == 0)
                {
                    productGroup.ProductGroupToPartRelations.Clear();
                }
                else
                {
                    var parts = await dbContext.Parts
                        .Where(e => monikers.Contains(e.Moniker))
                        .ToListAsync(CancellationToken.None);

                    if (monikers.Count != parts.Count)
                    {
                        throw new BadRequestException(problemDetailsFactory.BadRequest(
                            "Invalid Part.",
                            "Request contains one or more invalid Parts."));
                    }

                    productGroup.ProductGroupToPartRelations.Clear();
                    productGroup.ProductGroupToPartRelations.AddRange(parts.Select(e => new ProductGroupToPartRelation
                    {
                        ProductGroupId = productGroup.Id,
                        PartId = e.Id,
                    }));
                }

                await dbContext.SaveChangesAsync(CancellationToken.None);
            }
        }

        public class Validator : AbstractValidator<UpdateRelatedPartsCommand>
        {
            public Validator()
            {
                RuleFor(r => r.Moniker).NotEmpty();
                RuleForEach(r => r.PartMonikers).NotEmpty();
            }
        }
    }
}