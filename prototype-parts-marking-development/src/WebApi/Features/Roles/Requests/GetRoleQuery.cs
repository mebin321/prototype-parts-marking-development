namespace WebApi.Features.Roles.Requests
{
    using System.Threading;
    using System.Threading.Tasks;
    using FluentValidation;
    using MediatR;
    using Microsoft.EntityFrameworkCore;
    using Utilities;
    using WebApi.Data;
    using WebApi.Features.Roles.Models;

    public class GetRoleQuery : IRequest<RoleDto>
    {
        public string Moniker { get; set; }

        public class Handler : IRequestHandler<GetRoleQuery, RoleDto>
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

            public async Task<RoleDto> Handle(GetRoleQuery request, CancellationToken cancellationToken)
            {
                await using var dbContext = dbContextFactory.CreateDbContext();

                var role = await dbContext.Roles
                    .AsNoTracking()
                    .FirstOrDefaultAsync(r => r.Moniker == request.Moniker, CancellationToken.None)
                    .ThrowIfNullAsync(problemDetailsFactory.EntityNotFound(nameof(Role), request.Moniker));

                return RoleDto.From(role);
            }
        }

        public class Validator : AbstractValidator<GetRoleQuery>
        {
            public Validator()
            {
                RuleFor(r => r.Moniker).NotEmpty();
            }
        }
    }
}