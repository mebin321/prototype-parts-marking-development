namespace WebApi.Features.Prototypes.Requests
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using Microsoft.EntityFrameworkCore;
    using Utilities;
    using WebApi.Common.ResourceVersioning;
    using WebApi.Data;

    public class UpdatePrototypeCommand : IRequest
    {
        public int SetId { get; set; }

        public int PrototypeId { get; set; }

        public int OwnerId { get; set; }

        public string Comment { get; set; }

        public string MaterialNumber { get; set; }

        public string RevisionCode { get; set; }

        public class Handler : AsyncRequestHandler<UpdatePrototypeCommand>
        {
            private readonly IDbContextFactory<PrototypePartsDbContext> dbContextFactory;
            private readonly IProblemDetailsFactory problemDetailsFactory;
            private readonly IResourceVersionManager resourceVersionManager;

            public Handler(IDbContextFactory<PrototypePartsDbContext> dbContextFactory, IProblemDetailsFactory problemDetailsFactory, IResourceVersionManager resourceVersionManager)
            {
                Guard.NotNull(dbContextFactory, nameof(dbContextFactory));
                Guard.NotNull(problemDetailsFactory, nameof(problemDetailsFactory));
                Guard.NotNull(resourceVersionManager, nameof(resourceVersionManager));

                this.dbContextFactory = dbContextFactory;
                this.problemDetailsFactory = problemDetailsFactory;
                this.resourceVersionManager = resourceVersionManager;
            }

            protected override async Task Handle(UpdatePrototypeCommand request, CancellationToken cancellationToken)
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

                var prototype = set.Prototypes[0];
                resourceVersionManager.CheckVersion(prototype, true);

                var owner = await dbContext.Users
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.Id == request.OwnerId, CancellationToken.None);

                if (owner is null)
                {
                    throw new BadRequestException(problemDetailsFactory.BadRequest(
                        "Invalid Owner ID",
                        $"User with ID = {request.OwnerId} does not exist."));
                }

                prototype.OwnerId = owner.Id;
                prototype.Comment = request.Comment;
                prototype.MaterialNumber = request.MaterialNumber;
                prototype.RevisionCode = request.RevisionCode;

                await dbContext.SaveChangesAsync(CancellationToken.None);
            }
        }
    }
}