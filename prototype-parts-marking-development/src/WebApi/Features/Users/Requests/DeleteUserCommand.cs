namespace WebApi.Features.Users.Requests
{
    using System.Threading;
    using System.Threading.Tasks;
    using Data;
    using FluentValidation;
    using MediatR;
    using Microsoft.EntityFrameworkCore;
    using Utilities;

    public class DeleteUserCommand : IRequest
    {
        public int UserId { get; set; }

        public class Handler : AsyncRequestHandler<DeleteUserCommand>
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

            protected override async Task Handle(DeleteUserCommand request, CancellationToken cancellationToken)
            {
                await using var dbContext = dbContextFactory.CreateDbContext();

                var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == request.UserId);

                if (user is null)
                {
                    throw new NotFoundException(problemDetailsFactory.NotFound(
                        "User not found.",
                        $"Could not find user with ID {request.UserId}."));
                }

                dbContext.Users.Remove(user);
                await dbContext.SaveChangesAsync(cancellationToken);
            }
        }

        public class Validator : AbstractValidator<DeleteUserCommand>
        {
            public Validator()
            {
                RuleFor(r => r.UserId).GreaterThan(0);
            }
        }
    }
}