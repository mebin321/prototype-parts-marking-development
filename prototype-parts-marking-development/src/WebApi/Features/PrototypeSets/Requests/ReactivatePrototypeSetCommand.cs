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

    public class ReactivatePrototypeSetCommand : IRequest
    {
        public int SetId { get; set; }

        public class Handler : AsyncRequestHandler<ReactivatePrototypeSetCommand>
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

            protected override async Task Handle(ReactivatePrototypeSetCommand request, CancellationToken cancellationToken)
            {
                await using var dbContext = dbContextFactory.CreateDbContext();

                var prototypeSet = await dbContext.PrototypeSets
                    .FirstOrDefaultAsync(s => s.Id == request.SetId, CancellationToken.None)
                    .ThrowIfNullAsync(problemDetailsFactory.EntityNotFound(nameof(PrototypeSet), request.SetId));

                if (prototypeSet.DeletedAt is not null)
                {
                    prototypeSet.DeletedAt = null;
                    prototypeSet.DeletedById = null;
                    prototypeSet.ModifiedById = currentUserAccessor.GetCurrentUser();

                    await dbContext.SaveChangesAsync(CancellationToken.None);
                }
            }
        }

        public class Validator : AbstractValidator<ReactivatePrototypeSetCommand>
        {
            public Validator()
            {
                RuleFor(r => r.SetId).GreaterThan(0);
            }
        }
    }
}