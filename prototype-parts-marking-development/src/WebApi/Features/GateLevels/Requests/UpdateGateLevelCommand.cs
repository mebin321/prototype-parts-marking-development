namespace WebApi.Features.GateLevels.Requests
{
    using System.Threading;
    using System.Threading.Tasks;
    using Data;
    using FluentValidation;
    using MediatR;
    using Microsoft.EntityFrameworkCore;
    using Models;
    using Utilities;

    public class UpdateGateLevelCommand : IRequest<GateLevelDto>
    {
        public string Moniker { get; set; }

        public string Description { get; set; }

        public class Handler : IRequestHandler<UpdateGateLevelCommand, GateLevelDto>
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

            public async Task<GateLevelDto> Handle(UpdateGateLevelCommand request, CancellationToken cancellationToken)
            {
                await using var dbContext = dbContextFactory.CreateDbContext();

                var gateLevel = await dbContext.GateLevels
                    .FirstOrDefaultAsync(l => l.Moniker == request.Moniker);

                if (gateLevel is null)
                {
                    throw new NotFoundException(problemDetailsFactory.NotFound(
                        "Gate Level not found.",
                        $"Could not find Gate Level with ID {request.Moniker}."));
                }

                gateLevel.Description = request.Description;
                await dbContext.SaveChangesAsync();

                return GateLevelDto.From(gateLevel);
            }
        }

        public class Validator : AbstractValidator<UpdateGateLevelCommand>
        {
            public Validator()
            {
                RuleFor(r => r.Moniker).NotEmpty();
                RuleFor(r => r.Description).NotEmpty();
            }
        }
    }
}