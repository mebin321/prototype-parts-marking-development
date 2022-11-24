namespace WebApi.Features.Prototypes.Requests
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Authentication.Services;
    using Data;
    using FluentValidation;
    using MediatR;
    using Microsoft.EntityFrameworkCore;
    using Utilities;

    public class ScrapPrototypeCommand : IRequest
    {
        public int SetId { get; set; }

        public int PrototypeId { get; set; }

        public class Handler : AsyncRequestHandler<ScrapPrototypeCommand>
        {
            private readonly IDbContextFactory<PrototypePartsDbContext> dbContextFactory;
            private readonly IProblemDetailsFactory problemDetailsFactory;
            private readonly ICurrentUserAccessor currentUserAccessor;

            public Handler(IDbContextFactory<PrototypePartsDbContext> dbContextFactory, IProblemDetailsFactory problemDetailsFactory, ICurrentUserAccessor currentUserAccessor)
            {
                Guard.NotNull(dbContextFactory, nameof(dbContextFactory));
                Guard.NotNull(problemDetailsFactory, nameof(problemDetailsFactory));
                Guard.NotNull(currentUserAccessor, nameof(currentUserAccessor));

                this.dbContextFactory = dbContextFactory;
                this.problemDetailsFactory = problemDetailsFactory;
                this.currentUserAccessor = currentUserAccessor;
            }

            protected override async Task Handle(ScrapPrototypeCommand request, CancellationToken cancellationToken)
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

                if (prototype.DeletedAt is null)
                {
                    var currentUser = await GetUserAsync(dbContext, currentUserAccessor.GetCurrentUser());

                    prototype.DeletedById = currentUser.Id;
                    prototype.ModifiedById = currentUser.Id;

                    dbContext.Prototypes.Remove(prototype);

                    await dbContext.SaveChangesAsync(CancellationToken.None);
                }
            }

            private async Task<User> GetUserAsync(PrototypePartsDbContext dbContext, int id)
            {
                return await dbContext.Users
                    .SingleOrDefaultAsync(u => u.Id == id, CancellationToken.None)
                    .ThrowIfNullAsync(problemDetailsFactory.EntityNotFound(nameof(User), id));
            }
        }

        public class Validator : AbstractValidator<ScrapPrototypeCommand>
        {
            public Validator()
            {
                RuleFor(r => r.SetId).GreaterThan(0);
                RuleFor(r => r.PrototypeId).GreaterThan(0);
            }
        }
    }
}