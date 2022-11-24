namespace WebApi.Features.Users.Requests
{
    using System.Threading;
    using System.Threading.Tasks;
    using Data;
    using FluentValidation;
    using MediatR;
    using Microsoft.EntityFrameworkCore;
    using Utilities;

    public class ReactivateUserCommand : IRequest
    {
        public int UserId { get; set; }

        public class Handler : AsyncRequestHandler<ReactivateUserCommand>
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

            protected override async Task Handle(ReactivateUserCommand request, CancellationToken cancellationToken)
            {
                await using var dbContext = dbContextFactory.CreateDbContext();

                var user = await dbContext.Users
                    .FirstOrDefaultAsync(u => u.Id == request.UserId, CancellationToken.None)
                    .ThrowIfNullAsync(problemDetailsFactory.EntityNotFound(nameof(User), request.UserId));

                if (user.DeletedAt is not null)
                {
                    user.DeletedAt = null;

                    await dbContext.SaveChangesAsync(CancellationToken.None);
                }
            }
        }

        public class Validator : AbstractValidator<ReactivateUserCommand>
        {
            public Validator()
            {
                RuleFor(r => r.UserId).GreaterThan(0);
            }
        }
    }
}