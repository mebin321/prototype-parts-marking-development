namespace WebApi.Features.ProductGroup.Requests
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using FluentValidation;
    using MediatR;
    using Microsoft.EntityFrameworkCore;
    using Utilities;
    using WebApi.Data;

    public class UpdateRelatedProductGroupsCommand : IRequest
    {
        public string Moniker { get; set; }

        public string[] ProductGroupMonikers { get; set; }

        public class Handler : AsyncRequestHandler<UpdateRelatedProductGroupsCommand>
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

            protected override async Task Handle(UpdateRelatedProductGroupsCommand request, CancellationToken cancellationToken)
            {
                await using var dbContext = dbContextFactory.CreateDbContext();

                var outlet = await dbContext.Outlets
                    .Include(e => e.OutletToProductGroupRelations)
                    .FirstOrDefaultAsync(e => e.Moniker == request.Moniker, CancellationToken.None)
                    .ThrowIfNullAsync(problemDetailsFactory.EntityNotFound(nameof(Outlet), request.Moniker));

                var monikers = request.ProductGroupMonikers.Distinct().ToList();

                if (monikers.Count == 0)
                {
                    outlet.OutletToProductGroupRelations.Clear();
                }
                else
                {
                    var productGroups = await dbContext.ProductGroups
                        .Where(e => monikers.Contains(e.Moniker))
                        .ToListAsync(CancellationToken.None);

                    if (monikers.Count != productGroups.Count)
                    {
                        throw new BadRequestException(problemDetailsFactory.BadRequest(
                            "Invalid Product Group.",
                            "Request contains one or more invalid Product Groups."));
                    }

                    outlet.OutletToProductGroupRelations.Clear();
                    outlet.OutletToProductGroupRelations.AddRange(productGroups.Select(e => new OutletToProductGroupRelation
                    {
                        OutletId = outlet.Id,
                        ProductGroupId = e.Id,
                    }));
                }

                await dbContext.SaveChangesAsync(CancellationToken.None);
            }
        }

        public class Validator : AbstractValidator<UpdateRelatedProductGroupsCommand>
        {
            public Validator()
            {
                RuleFor(r => r.Moniker).NotEmpty();
                RuleForEach(r => r.ProductGroupMonikers).NotEmpty();
            }
        }
    }
}