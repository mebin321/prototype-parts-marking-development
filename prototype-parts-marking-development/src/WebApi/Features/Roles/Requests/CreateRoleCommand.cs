namespace WebApi.Features.Roles.Requests
{
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using FluentValidation;
    using MediatR;
    using Microsoft.EntityFrameworkCore;
    using Utilities;
    using WebApi.Common;
    using WebApi.Data;
    using WebApi.Features.Roles.Models;

    public class CreateRoleCommand : IRequest<RoleDto>
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        public class Handler : IRequestHandler<CreateRoleCommand, RoleDto>
        {
            private readonly IDbContextFactory<PrototypePartsDbContext> dbContextFactory;
            private readonly IProblemDetailsFactory problemDetailsFactory;
            private readonly IMonikerFormatter monikerFormatter;

            public Handler(
                IDbContextFactory<PrototypePartsDbContext> dbContextFactory,
                IProblemDetailsFactory problemDetailsFactory,
                IMonikerFormatter monikerFormatter)
            {
                Guard.NotNull(dbContextFactory, nameof(dbContextFactory));
                Guard.NotNull(problemDetailsFactory, nameof(problemDetailsFactory));
                Guard.NotNull(monikerFormatter, nameof(monikerFormatter));

                this.dbContextFactory = dbContextFactory;
                this.problemDetailsFactory = problemDetailsFactory;
                this.monikerFormatter = monikerFormatter;
            }

            public async Task<RoleDto> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
            {
                await using var dbContext = dbContextFactory.CreateDbContext();

                var moniker = monikerFormatter.Format(request.Title);

                var existing = dbContext.Roles
                    .AsNoTracking()
                    .Count(r => r.Moniker == moniker);

                if (existing != 0)
                {
                    throw new BadRequestException(problemDetailsFactory.BadRequest(
                        "Invalid Role.",
                        "Provided Role already exists. Roles must be unique."));
                }

                var role = new Role
                {
                    Moniker = moniker,
                    Title = request.Title,
                    Description = request.Description,
                };
                dbContext.Roles.Add(role);
                await dbContext.SaveChangesAsync();

                return RoleDto.From(role);
            }
        }

        public class Validator : AbstractValidator<CreateRoleCommand>
        {
            public Validator()
            {
                RuleFor(r => r.Title).NotEmpty();
                RuleFor(r => r.Description).NotEmpty();
            }
        }
    }
}