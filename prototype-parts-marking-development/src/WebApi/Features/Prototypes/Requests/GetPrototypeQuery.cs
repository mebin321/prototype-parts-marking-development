namespace WebApi.Features.Prototypes.Requests
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Common.ResourceVersioning;
    using Data;
    using FluentValidation;
    using MediatR;
    using Microsoft.EntityFrameworkCore;
    using Models;
    using Utilities;

    public class GetPrototypeQuery : IRequest<PrototypeDto>
    {
        public int PrototypeId { get; set; }

        public int SetId { get; set; }

        public class Handler : IRequestHandler<GetPrototypeQuery, PrototypeDto>
        {
            private readonly IDbContextFactory<PrototypePartsDbContext> dbContextFactory;
            private readonly IProblemDetailsFactory problemDetailsFactory;
            private readonly IResourceVersionManager resourceVersionManager;

            public Handler(
                IDbContextFactory<PrototypePartsDbContext> dbContextFactory,
                IProblemDetailsFactory problemDetailsFactory,
                IResourceVersionManager resourceVersionManager)
            {
                Guard.NotNull(dbContextFactory, nameof(dbContextFactory));
                Guard.NotNull(problemDetailsFactory, nameof(problemDetailsFactory));
                Guard.NotNull(resourceVersionManager, nameof(resourceVersionManager));

                this.dbContextFactory = dbContextFactory;
                this.problemDetailsFactory = problemDetailsFactory;
                this.resourceVersionManager = resourceVersionManager;
            }

            public async Task<PrototypeDto> Handle(GetPrototypeQuery request, CancellationToken cancellationToken)
            {
                await using var dbContext = dbContextFactory.CreateDbContext();

                if (!dbContext.PrototypeSets.Any(s => s.Id == request.SetId))
                {
                    throw new NotFoundException(
                        problemDetailsFactory.NotFound(
                            "PrototypeSet not found.",
                            $"Could not find PrototypeSet with Id {request.SetId}."));
                }

                var prototype = await dbContext.Prototypes
                    .AsNoTracking()
                    .Include(s => s.Owner)
                    .Include(s => s.CreatedBy)
                    .Include(s => s.ModifiedBy)
                    .Include(s => s.DeletedBy)
                    .FirstOrDefaultAsync(p => p.PrototypeSetId == request.SetId && p.Id == request.PrototypeId);

                if (prototype is null)
                {
                    throw new NotFoundException(problemDetailsFactory.NotFound(
                        "Prototype not found.",
                        $"Could not find Prototype with ID {request.PrototypeId}."));
                }

                resourceVersionManager.SetEtag(prototype);

                return PrototypeDto.From(prototype);
            }
        }

        public class Validator : AbstractValidator<GetPrototypeQuery>
        {
            public Validator()
            {
                RuleFor(r => r.PrototypeId).NotEmpty();
                RuleFor(r => r.SetId).NotEmpty();
            }
        }
    }
}
