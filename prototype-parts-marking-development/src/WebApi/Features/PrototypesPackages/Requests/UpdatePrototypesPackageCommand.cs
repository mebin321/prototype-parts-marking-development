namespace WebApi.Features.PrototypesPackages.Requests
{
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using Microsoft.EntityFrameworkCore;
    using Utilities;
    using WebApi.Common.ResourceVersioning;
    using WebApi.Data;

    public class UpdatePrototypesPackageCommand : IRequest
    {
        public int Id { get; set; }

        public int OwnerId { get; set; }

        public string Commment { get; set; }

        public int ActualCount { get; set; }

        public class Handler : AsyncRequestHandler<UpdatePrototypesPackageCommand>
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

            protected override async Task Handle(UpdatePrototypesPackageCommand request, CancellationToken cancellationToken)
            {
                await using var dbContext = dbContextFactory.CreateDbContext();

                var package = await dbContext.PrototypesPackages
                    .FirstOrDefaultAsync(p => p.Id == request.Id, CancellationToken.None)
                    .ThrowIfNullAsync(problemDetailsFactory.EntityNotFound(nameof(PrototypesPackage), request.Id));

                resourceVersionManager.CheckVersion(package, true);

                var owner = await dbContext.Users
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.Id == request.OwnerId, CancellationToken.None);

                if (owner is null)
                {
                    throw new BadRequestException(problemDetailsFactory.BadRequest(
                        "Invalid Owner ID",
                        $"User with ID = {request.OwnerId} does not exist."));
                }

                package.OwnerId = owner.Id;
                package.Comment = request.Commment;
                package.ActualCount = request.ActualCount;

                await dbContext.SaveChangesAsync(CancellationToken.None);
            }
        }
    }
}