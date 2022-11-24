namespace WebApi.Features.Roles.Requests
{
    using System.Threading;
    using System.Threading.Tasks;
    using FluentValidation;
    using MediatR;
    using Microsoft.EntityFrameworkCore;
    using Utilities;
    using WebApi.Common;
    using WebApi.Data;
    using WebApi.Features.Roles.Models;

    public class UpdateRoleCommand : IRequest<RoleDto>
    {
        public string Moniker { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public class Handler : IRequestHandler<UpdateRoleCommand, RoleDto>
        {
            private readonly IDbContextFactory<PrototypePartsDbContext> dbContextFactory;
            private readonly IProblemDetailsFactory problemDetailsFactory;
            private readonly IMonikerFormatter monikerFormatter;

            public Handler(IDbContextFactory<PrototypePartsDbContext> dbContextFactory, IProblemDetailsFactory problemDetailsFactory, IMonikerFormatter monikerFormatter)
            {
                Guard.NotNull(dbContextFactory, nameof(dbContextFactory));
                Guard.NotNull(problemDetailsFactory, nameof(problemDetailsFactory));
                Guard.NotNull(monikerFormatter, nameof(monikerFormatter));

                this.dbContextFactory = dbContextFactory;
                this.problemDetailsFactory = problemDetailsFactory;
                this.monikerFormatter = monikerFormatter;
            }

            public async Task<RoleDto> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
            {
                await using var dbContext = dbContextFactory.CreateDbContext();

                var role = await dbContext.Roles
                    .FirstOrDefaultAsync(r => r.Moniker == request.Moniker, CancellationToken.None)
                    .ThrowIfNullAsync(problemDetailsFactory.EntityNotFound(nameof(Role), request.Moniker));

                role.Moniker = monikerFormatter.Format(request.Title);
                role.Title = request.Title;
                role.Description = request.Description;

                await dbContext.SaveChangesAsync();

                return RoleDto.From(role);
            }
        }

        public class Validator : AbstractValidator<UpdateRoleCommand>
        {
            public Validator()
            {
                RuleFor(r => r.Title).NotEmpty();
                RuleFor(r => r.Description).NotEmpty();
            }
        }
    }
}