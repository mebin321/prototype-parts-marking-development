namespace WebApi.Features.Locations.Requests
{
    using System.Threading;
    using System.Threading.Tasks;
    using Data;
    using FluentValidation;
    using MediatR;
    using Microsoft.EntityFrameworkCore;
    using Models;
    using Utilities;

    public class GetLocationQuery : IRequest<LocationDto>
    {
        public string Moniker { get; set; }

        public class Handler : IRequestHandler<GetLocationQuery, LocationDto>
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

            public async Task<LocationDto> Handle(GetLocationQuery request, CancellationToken cancellationToken)
            {
                await using var dbContext = dbContextFactory.CreateDbContext();

                var location = await dbContext.Locations
                    .AsNoTracking()
                    .FirstOrDefaultAsync(l => l.Moniker == request.Moniker, CancellationToken.None)
                    .ThrowIfNullAsync(problemDetailsFactory.EntityNotFound(nameof(Location), request.Moniker));

                return LocationDto.From(location);
            }
        }

        public class Validator : AbstractValidator<GetLocationQuery>
        {
            public Validator()
            {
                RuleFor(r => r.Moniker).NotEmpty();
            }
        }
    }
}
