namespace WebApi.Features.Maintenance
{
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using FluentValidation;
    using MediatR;
    using Microsoft.EntityFrameworkCore;
    using Swashbuckle.AspNetCore.Annotations;
    using Utilities;
    using WebApi.Data;

    public class RevokeRefreshTokenCommand : IRequest
    {
        [Required]
        [SwaggerSchema("Refresh token to be revoked.")]
        public string Token { get; set; }

        public class Handler : AsyncRequestHandler<RevokeRefreshTokenCommand>
        {
            private readonly IDbContextFactory<PrototypePartsDbContext> dbContextFactory;

            public Handler(IDbContextFactory<PrototypePartsDbContext> dbContextFactory)
            {
                Guard.NotNull(dbContextFactory, nameof(dbContextFactory));

                this.dbContextFactory = dbContextFactory;
            }

            protected override async Task Handle(RevokeRefreshTokenCommand request, CancellationToken cancellationToken)
            {
                await using var dbContext = dbContextFactory.CreateDbContext();

                var user = await dbContext.Users
                    .Include(u => u.RefreshTokens)
                    .FirstOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == request.Token));

                if (user is null)
                {
                    return;
                }

                var token = user.RefreshTokens.First(t => t.Token == request.Token);
                user.RefreshTokens.Remove(token);

                await dbContext.SaveChangesAsync();
            }
        }

        public class Validator : AbstractValidator<RevokeRefreshTokenCommand>
        {
            public Validator()
            {
                RuleFor(r => r.Token).NotEmpty();
            }
        }
    }
}