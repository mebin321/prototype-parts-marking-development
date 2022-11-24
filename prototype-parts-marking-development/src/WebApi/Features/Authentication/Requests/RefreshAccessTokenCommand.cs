namespace WebApi.Features.Authentication.Requests
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using FluentValidation;
    using MediatR;
    using Microsoft.EntityFrameworkCore;
    using Utilities;
    using WebApi.Common;
    using WebApi.Data;
    using WebApi.Features.Authentication.Models;
    using WebApi.Features.Authentication.Services;
    using WebApi.Features.Users.Models;

    public class RefreshAccessTokenCommand : IRequest<AuthenticationResponseDto>
    {
        public string Token { get; set; }

        public class Handler : IRequestHandler<RefreshAccessTokenCommand, AuthenticationResponseDto>
        {
            private readonly IDbContextFactory<PrototypePartsDbContext> dbContextFactory;
            private readonly ITokenGenerator tokenGenerator;
            private readonly IProblemDetailsFactory problemDetailsFactory;
            private readonly IDateTime dateTime;

            public Handler(
                IDbContextFactory<PrototypePartsDbContext> dbContextFactory,
                ITokenGenerator tokenGenerator,
                IProblemDetailsFactory problemDetailsFactory,
                IDateTime dateTime)
            {
                Guard.NotNull(dbContextFactory, nameof(dbContextFactory));
                Guard.NotNull(tokenGenerator, nameof(tokenGenerator));
                Guard.NotNull(problemDetailsFactory, nameof(problemDetailsFactory));
                Guard.NotNull(dateTime, nameof(dateTime));

                this.dbContextFactory = dbContextFactory;
                this.tokenGenerator = tokenGenerator;
                this.problemDetailsFactory = problemDetailsFactory;
                this.dateTime = dateTime;
            }

            public async Task<AuthenticationResponseDto> Handle(RefreshAccessTokenCommand request, CancellationToken cancellationToken)
            {
                await using var dbContext = dbContextFactory.CreateDbContext();

                var user = await dbContext.Users
                    .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                    .Include(u => u.RefreshTokens)
                    .Where(u => u.DeletedAt == null)
                    .FirstOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == request.Token));

                if (user is null)
                {
                    throw new BadRequestException(problemDetailsFactory.BadRequest(
                        "Invalid refresh token",
                        "The provided refresh token is invalid."));
                }

                var token = user.RefreshTokens.First(t => t.Token == request.Token);
                if (token.ExpiresAt <= dateTime.Now)
                {
                    throw new BadRequestException(problemDetailsFactory.BadRequest(
                        "Invalid refresh token",
                        "The provided refresh token has expired."));
                }

                var accessToken = tokenGenerator.CreateAccessToken(
                    user.Id,
                    user.UserRoles.Select(ur => ur.Role).Select(r => r.Moniker));
                var refreshToken = tokenGenerator.CreateRefreshToken();

                user.RefreshTokens.Remove(token);
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

        public class Validator : AbstractValidator<RefreshAccessTokenCommand>
        {
            public Validator()
            {
                RuleFor(r => r.Token).NotEmpty();
            }
        }
    }
}