namespace WebApi.Features.Roles.Requests
{
    using System.Threading;
    using System.Threading.Tasks;
    using FluentValidation;
    using MediatR;
    using Microsoft.EntityFrameworkCore;
    using Utilities;
    using WebApi.Data;

    public class DeleteRoleCommand : IRequest
    {
        public string Moniker { get; set; }

        public class Handler : AsyncRequestHandler<DeleteRoleCommand>
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

            protected override async Task Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
            {
                await using var dbContext = dbContextFactory.CreateDbContext();

                var role = await dbContext.Roles
                    .FirstOrDefaultAsync(r => r.Moniker == request.Moniker, CancellationToken.None)
                    .ThrowIfNullAsync(problemDetailsFactory.EntityNotFound(nameof(Role), request.Moniker));

                dbContext.Roles.Remove(role);

                await dbContext.SaveChangesAsync();
            }
        }

        public class Validator : AbstractValidator<DeleteRoleCommand>
        {
            public Validator()
            {
                RuleFor(r => r.Moniker).NotEmpty();
            }
        }
    }
}
