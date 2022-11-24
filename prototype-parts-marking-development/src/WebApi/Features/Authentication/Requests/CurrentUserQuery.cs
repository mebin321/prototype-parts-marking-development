namespace WebApi.Features.Authentication.Requests
{
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using Microsoft.EntityFrameworkCore;
    using Utilities;
    using WebApi.Data;
    using WebApi.Features.Authentication.Services;
    using WebApi.Features.Users.Models;

    public class CurrentUserQuery : IRequest<UserDto>
    {
        public class Handler : IRequestHandler<CurrentUserQuery, UserDto>
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

            public async Task<UserDto> Handle(CurrentUserQuery request, CancellationToken cancellationToken)
            {
                await using var dbContext = dbContextFactory.CreateDbContext();

                var user = await dbContext.Users
                    .AsNoTracking()
                    .FirstOrDefaultAsync(
                        u => u.Id == currentUserAccessor.GetCurrentUser(),
                        CancellationToken.None);

                if (user is null)
                {
                    throw new NotFoundException(problemDetailsFactory.NotFound(
                        "User not found",
                        "Could not find user with the provided ID."));
                }

                return new UserDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    Name = user.Name,
                    Username = user.DomainIdentity,
                };
            }
        }
    }
}