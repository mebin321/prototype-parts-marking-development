namespace WebApi.Features.Outlets.Requests
{
    using System.Threading;
    using System.Threading.Tasks;
    using Data;
    using FluentValidation;
    using MediatR;
    using Microsoft.EntityFrameworkCore;
    using Models;
    using Utilities;

    public class GetOutletQuery : IRequest<OutletDto>
    {
        public string Moniker { get; set; }

        public class Handler : IRequestHandler<GetOutletQuery, OutletDto>
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

            public async Task<OutletDto> Handle(GetOutletQuery request, CancellationToken cancellationToken)
            {
                await using var dbContext = dbContextFactory.CreateDbContext();

                var outlet = await dbContext.Outlets
                    .AsNoTracking()
                    .FirstOrDefaultAsync(l => l.Moniker == request.Moniker);

                if (outlet is null)
                {
                    throw new NotFoundException(problemDetailsFactory.NotFound(
                        "Outlet not found.",
                        $"Could not find Outlet with ID {request.Moniker}."));
                }

                return OutletDto.From(outlet);
            }
        }

        public class Validator : AbstractValidator<GetOutletQuery>
        {
            public Validator()
            {
                RuleFor(r => r.Moniker).NotEmpty();
            }
        }
    }
}
