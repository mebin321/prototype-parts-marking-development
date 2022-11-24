namespace WebApi.Features.PrototypeSets.Requests
{
    using System.Threading;
    using System.Threading.Tasks;
    using Authentication.Services;
    using Data;
    using FluentValidation;
    using MediatR;
    using Microsoft.EntityFrameworkCore;
    using Utilities;

    public class ScrapPrototypeSetCommand : IRequest
    {
        public int SetId { get; set; }

        public class Handler : AsyncRequestHandler<ScrapPrototypeSetCommand>
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

            protected override async Task Handle(ScrapPrototypeSetCommand request, CancellationToken cancellationToken)
            {
                await using var dbContext = dbContextFactory.CreateDbContext();

                var prototypeSet = await dbContext.PrototypeSets
                    .FirstOrDefaultAsync(s => s.Id == request.SetId, CancellationToken.None)
                    .ThrowIfNullAsync(problemDetailsFactory.EntityNotFound(nameof(PrototypeSet), request.SetId));

                if (prototypeSet.DeletedAt is null)
                {
                    var currentUser = currentUserAccessor.GetCurrentUser();

                    prototypeSet.DeletedById = currentUser;
                    prototypeSet.ModifiedById = currentUser;

                    dbContext.PrototypeSets.Remove(prototypeSet);

                    await dbContext.SaveChangesAsync(CancellationToken.None);
                }
            }
        }

        public class Validator : AbstractValidator<ScrapPrototypeSetCommand>
        {
            public Validator()
            {
                RuleFor(r => r.SetId).GreaterThan(0);
            }
        }
    }
}