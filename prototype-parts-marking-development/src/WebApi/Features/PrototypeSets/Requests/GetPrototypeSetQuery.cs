namespace WebApi.Features.PrototypeSets.Requests
{
    using System.Threading;
    using System.Threading.Tasks;
    using Data;
    using FluentValidation;
    using MediatR;
    using Microsoft.EntityFrameworkCore;
    using Models;
    using Utilities;
    using WebApi.Common.ResourceVersioning;

    public class GetPrototypeSetQuery : IRequest<PrototypeSetDto>
    {
        public int Id { get; set; }

        public class Handler : IRequestHandler<GetPrototypeSetQuery, PrototypeSetDto>
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

                this.dbContextFactory = dbContextFactory;
                this.problemDetailsFactory = problemDetailsFactory;
                this.resourceVersionManager = resourceVersionManager;
            }

            public async Task<PrototypeSetDto> Handle(
                GetPrototypeSetQuery request,
                CancellationToken cancellationToken)
            {
                await using var dbContext = dbContextFactory.CreateDbContext();

                var prototypeSet = await dbContext.PrototypeSets
                    .AsNoTracking()
                    .Include(s => s.CreatedBy)
                    .Include(s => s.ModifiedBy)
                    .Include(s => s.DeletedBy)
                    .SingleOrDefaultAsync(s => s.Id == request.Id);

                if (prototypeSet is null)
                {
                    throw new NotFoundException(problemDetailsFactory.NotFound(
                        "PrototypeSet not found.",
                        $"Could not find PrototypeSet with ID {request.Id}."));
                }

                resourceVersionManager.SetEtag(prototypeSet);

                return PrototypeSetDto.From(prototypeSet);
            }
        }

        public class Validator : AbstractValidator<GetPrototypeSetQuery>
        {
            public Validator()
            {
                RuleFor(u => u.Id).GreaterThan(0);
            }
        }
    }
}