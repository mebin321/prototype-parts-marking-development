namespace WebApi.Features.PrototypeVariants.Requests
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Data;
    using FluentValidation;
    using MediatR;
    using Microsoft.EntityFrameworkCore;
    using Models;
    using Utilities;

    public class GetPrototypeVariantQuery : IRequest<PrototypeVariantDto>
    {
        public int SetId { get; set; }

        public int PrototypeId { get; set; }

        public int VariantVersion { get; set; }

        public class Handler : IRequestHandler<GetPrototypeVariantQuery, PrototypeVariantDto>
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

            public async Task<PrototypeVariantDto> Handle(GetPrototypeVariantQuery request, CancellationToken cancellationToken)
            {
                await using var dbContext = dbContextFactory.CreateDbContext();

                var set = await dbContext.PrototypeSets
                    .Include(s => s.Prototypes.Where(p => p.Id == request.PrototypeId))
                    .FirstOrDefaultAsync(s => s.Id == request.SetId, CancellationToken.None)
                    .ThrowIfNullAsync(problemDetailsFactory.EntityNotFound(nameof(PrototypeSet), request.SetId));

                if (set.Prototypes.Count == 0)
                {
                    throw problemDetailsFactory.EntityNotFound(nameof(Prototype), request.PrototypeId);
                }

                var variant = await dbContext.PrototypeVariants
                    .AsNoTracking()
                    .Include(v => v.CreatedBy)
                    .Include(v => v.ModifiedBy)
                    .Include(v => v.DeletedBy)
                    .FirstOrDefaultAsync(
                        v => v.PrototypeId == request.PrototypeId && v.Version == request.VariantVersion,
                        CancellationToken.None);

                if (variant is null)
                {
                    throw new NotFoundException(problemDetailsFactory.NotFound(
                        "PrototypeVariant not found.",
                        $"Could not find PrototypeVariant with Version {request.VariantVersion}."));
                }

                return PrototypeVariantDto.From(variant);
            }
        }

        public class Validator : AbstractValidator<GetPrototypeVariantQuery>
        {
            public Validator()
            {
                RuleFor(r => r.SetId).GreaterThan(0);
                RuleFor(r => r.PrototypeId).GreaterThan(0);
                RuleFor(r => r.VariantVersion).GreaterThan(0);
            }
        }
    }
}
