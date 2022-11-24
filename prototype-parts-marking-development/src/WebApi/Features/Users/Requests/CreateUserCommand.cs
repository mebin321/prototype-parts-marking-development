namespace WebApi.Features.Users.Requests
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Common.ActiveDirectory;
    using Data;
    using FluentValidation;
    using MediatR;
    using Microsoft.EntityFrameworkCore;
    using Models;
    using Utilities;

    public class CreateUserCommand : IRequest<UserDto>
    {
        public string Username { get; set; }

        public class Handler : IRequestHandler<CreateUserCommand, UserDto>
        {
            private readonly IDbContextFactory<PrototypePartsDbContext> dbContextFactory;
            private readonly IProblemDetailsFactory problemDetailsFactory;
            private readonly IActiveDirectory activeDirectory;

            public Handler(IDbContextFactory<PrototypePartsDbContext> dbContextFactory, IProblemDetailsFactory problemDetailsFactory, IActiveDirectory activeDirectory)
            {
                Guard.NotNull(dbContextFactory, nameof(dbContextFactory));
                Guard.NotNull(problemDetailsFactory, nameof(problemDetailsFactory));
                Guard.NotNull(activeDirectory, nameof(activeDirectory));

                this.dbContextFactory = dbContextFactory;
                this.problemDetailsFactory = problemDetailsFactory;
                this.activeDirectory = activeDirectory;
            }

            public async Task<UserDto> Handle(CreateUserCommand request, CancellationToken cancellationToken)
            {
                await using var dbContext = dbContextFactory.CreateDbContext();

                var existing = await dbContext.Users
                    .AsNoTracking()
                    .Where(u => EF.Functions.ILike(u.DomainIdentity, $"%{request.Username}%"))
                    .FirstOrDefaultAsync();

                if (existing != null)
                {
                    throw new BadRequestException(problemDetailsFactory.BadRequest(
                        "Invalid username",
                        "User with provided username already exists."));
                }

                var adUserDto = activeDirectory.FindUser(request.Username, out int count);

                if (adUserDto is null)
                {
                    throw new BadRequestException(problemDetailsFactory.BadRequest(
                        "Invalid username",
                        $"Found {count} AD users with the provided username."));
                }

                var user = new User()
                {
                    Name = adUserDto.Name,
                    Email = adUserDto.Email,
                    DomainIdentity = adUserDto.Username,
                    ServiceAccount = false,
                };

                dbContext.Users.Add(user);
                await dbContext.SaveChangesAsync();

                return UserDto.From(user);
            }
        }

        public class Validator : AbstractValidator<CreateUserCommand>
        {
            public Validator()
            {
                RuleFor(r => r.Username).NotEmpty();
            }
        }
    }
}