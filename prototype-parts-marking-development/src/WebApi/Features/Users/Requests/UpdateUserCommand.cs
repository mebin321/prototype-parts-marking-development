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

    public class UpdateUserCommand : IRequest<UserDto>
    {
        public int UserId { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public class Handler : IRequestHandler<UpdateUserCommand, UserDto>
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

            public async Task<UserDto> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
            {
                await using var dbContext = dbContextFactory.CreateDbContext();

                var user = await dbContext.Users.FindAsync(request.UserId);

                if (user is null)
                {
                    throw new NotFoundException(problemDetailsFactory.NotFound(
                        "User not found.",
                        $"Could not find user with ID = {request.UserId}"));
                }

                user.Name = request.Name;
                user.Email = request.Email;

                await dbContext.SaveChangesAsync();

                return new UserDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    Username = user.DomainIdentity,
                    Name = user.Name,
                };
            }
        }

        public class Validator : AbstractValidator<UpdateUserCommand>
        {
            public Validator()
            {
                RuleFor(r => r.Name).NotEmpty();
                RuleFor(r => r.Email)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty()
                    .EmailAddress();
            }
        }
    }
}
