namespace WebApi.Features.Users.Requests
{
    using System.Threading;
    using System.Threading.Tasks;
    using Data;
    using FluentValidation;
    using MediatR;
    using Microsoft.EntityFrameworkCore;
    using Models;
    using Utilities;

    public class GetUserQuery : IRequest<EnrichedUserDto>
    {
        public int UserId { get; set; }

        public class Handler : IRequestHandler<GetUserQuery, EnrichedUserDto>
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

            public async Task<EnrichedUserDto> Handle(GetUserQuery request, CancellationToken cancellationToken)
            {
                await using var dbContext = dbContextFactory.CreateDbContext();

                var user = await dbContext.Users
                    .AsNoTracking()
                    .SingleOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

                if (user is null)
                {
                    throw new NotFoundException(problemDetailsFactory.NotFound(
                        "User not found.",
                        $"Could not find user with ID = {request.UserId}"));
                }

                return EnrichedUserDto.From(user);
            }
        }

        public class Validator : AbstractValidator<GetUserQuery>
        {
            public Validator()
            {
                RuleFor(r => r.UserId).GreaterThan(0);
            }
        }
    }
}