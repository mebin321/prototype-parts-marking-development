namespace WebApi.Features.PrototypesPackages.Requests
{
    using System.Threading;
    using System.Threading.Tasks;
    using Authentication.Services;
    using Data;
    using FluentValidation;
    using MediatR;
    using Microsoft.EntityFrameworkCore;
    using Utilities;

    public class ReactivatePrototypesPackageCommand : IRequest
    {
        public int Id { get; set; }

        public class Handler : AsyncRequestHandler<ReactivatePrototypesPackageCommand>
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

            protected override async Task Handle(ReactivatePrototypesPackageCommand request, CancellationToken cancellationToken)
            {
                await using var dbContext = dbContextFactory.CreateDbContext();

                var prototypesPackage = await dbContext.PrototypesPackages
                    .FirstOrDefaultAsync(p => p.Id == request.Id, CancellationToken.None)
                    .ThrowIfNullAsync(problemDetailsFactory.EntityNotFound(nameof(PrototypesPackage), request.Id));

                if (prototypesPackage.DeletedAt is not null)
                {
                    var currentUser = currentUserAccessor.GetCurrentUser();

                    prototypesPackage.DeletedAt = null;
                    prototypesPackage.DeletedById = null;
                    prototypesPackage.ModifiedById = currentUser;

                    await dbContext.SaveChangesAsync(CancellationToken.None);
                }
            }
        }

        public class Validator : AbstractValidator<ReactivatePrototypesPackageCommand>
        {
            public Validator()
            {
                RuleFor(r => r.Id).GreaterThan(0);
            }
        }
    }
}