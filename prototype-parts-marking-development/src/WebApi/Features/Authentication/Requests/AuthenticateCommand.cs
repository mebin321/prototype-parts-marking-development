namespace WebApi.Features.Authentication.Requests
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using FluentValidation;
    using MediatR;
    using Microsoft.EntityFrameworkCore;
    using Utilities;
    using WebApi.Common.Logging;
    using WebApi.Data;
    using WebApi.Features.Authentication.Models;
    using WebApi.Features.Authentication.Services;
    using WebApi.Features.Users.Models;

    public class AuthenticateCommand : IRequest<AuthenticationResponseDto>, IDestructurable
    {
        public string Username { get; set; }

        public string Password { get; set; }

        object IDestructurable<object>.Destructure() => new
        {
            Username,
            Password = "*****",
        };

        public class Handler : IRequestHandler<AuthenticateCommand, AuthenticationResponseDto>
        {
            private readonly IDbContextFactory<PrototypePartsDbContext> dbContextFactory;
            private readonly ICredentialsValidator credentialsValidator;
            private readonly IProblemDetailsFactory problemDetailsFactory;
            private readonly ITokenGenerator tokenGenerator;

            public Handler(IDbContextFactory<PrototypePartsDbContext> dbContextFactory, ICredentialsValidator credentialsValidator, IProblemDetailsFactory problemDetailsFactory, ITokenGenerator tokenGenerator)
            {
                Guard.NotNull(dbContextFactory, nameof(dbContextFactory));
                Guard.NotNull(credentialsValidator, nameof(credentialsValidator));
                Guard.NotNull(problemDetailsFactory, nameof(problemDetailsFactory));
                Guard.NotNull(tokenGenerator, nameof(tokenGenerator));

                this.dbContextFactory = dbContextFactory;
                this.credentialsValidator = credentialsValidator;
                this.problemDetailsFactory = problemDetailsFactory;
                this.tokenGenerator = tokenGenerator;
            }

            public async Task<AuthenticationResponseDto> Handle(AuthenticateCommand request, CancellationToken cancellationToken)
            {
                await using var dbContext = dbContextFactory.CreateDbContext();

                var user = await dbContext.Users
                    .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                    .Where(u => u.DeletedAt == null)
                    .FirstOrDefaultAsync(u => EF.Functions.ILike(u.DomainIdentity, $"%{request.Username}%"));

                if (user is null || user.ServiceAccount)
                {
                    throw new NotAuthorizedException(problemDetailsFactory.Unauthorized(
                        "Authentication failed",
                        "Invalid username or password."));
                }

                if (!credentialsValidator.Validate(request.Username, request.Password))
                {
                    throw new NotAuthorizedException(problemDetailsFactory.Unauthorized(
                        "Authentication failed",
                        "Invalid username or password."));
                }

                var accessToken = tokenGenerator.CreateAccessToken(
                    user.Id,
                    user.UserRoles.Select(ur => ur.Role).Select(r => r.Moniker));
                var refreshToken = tokenGenerator.CreateRefreshToken();

                user.RefreshTokens.Add(new RefreshToken
                {
                    Token = refreshToken.Token,
                    CreatedAt = refreshToken.CreatedAt,
                    ExpiresAt = refreshToken.ExpiresAt,
                });
                await dbContext.SaveChangesAsync();

                return new AuthenticationResponseDto
                {
                    User = new UserDto
                    {
                        Id = user.Id,
                        Email = user.Email,
                        Name = user.Name,
                        Username = user.DomainIdentity,
                    },
                    AccessToken = new TokenDto
                    {
                        Token = accessToken.Token,
                        ExpiresAt = accessToken.ExpiresAt,
                    },
                    RefreshToken = new TokenDto
                    {
                        Token = refreshToken.Token,
                        ExpiresAt = refreshToken.ExpiresAt,
                    },
                };
            }
        }

        public class Validator : AbstractValidator<AuthenticateCommand>
        {
            public Validator()
            {
                RuleFor(r => r.Username).NotEmpty();
                RuleFor(r => r.Password).NotEmpty();
            }
        }
    }
}
