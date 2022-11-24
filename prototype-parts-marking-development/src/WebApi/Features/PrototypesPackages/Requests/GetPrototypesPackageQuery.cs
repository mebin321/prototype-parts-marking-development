namespace WebApi.Features.PrototypesPackages.Requests
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

    public class GetPrototypesPackageQuery : IRequest<PrototypesPackageDto>
    {
        public int Id { get; set; }

        public class Handler : IRequestHandler<GetPrototypesPackageQuery, PrototypesPackageDto>
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

            public async Task<PrototypesPackageDto> Handle(
                GetPrototypesPackageQuery request,
                CancellationToken cancellationToken)
            {
                await using var dbContext = dbContextFactory.CreateDbContext();

                var prototypesPackage = await dbContext.PrototypesPackages
                    .AsNoTracking()
                    .Include(p => p.Owner)
                    .Include(p => p.CreatedBy)
                    .Include(p => p.ModifiedBy)
                    .SingleOrDefaultAsync(p => p.Id == request.Id);

                if (prototypesPackage is null)
                {
                    throw new NotFoundException(problemDetailsFactory.NotFound(
                        "Prototypes package not found.",
                        $"Could not find prototypes package with ID {request.Id}."));
                }

                resourceVersionManager.SetEtag(prototypesPackage);

                return PrototypesPackageDto.From(prototypesPackage);
            }
        }

        public class Validator : AbstractValidator<GetPrototypesPackageQuery>
        {
            public Validator()
            {
                RuleFor(u => u.Id).GreaterThan(0);
            }
        }
    }
}