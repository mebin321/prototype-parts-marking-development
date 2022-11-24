namespace WebApi.Features.Locations.Requests
{
    using System.ComponentModel.DataAnnotations;
    using System.Threading;
    using System.Threading.Tasks;
    using Data;
    using FluentValidation;
    using MediatR;
    using Microsoft.EntityFrameworkCore;
    using Models;
    using Utilities;

    public class UpdateLocationCommand : IRequest<LocationDto>
    {
        [Required]
        public string Moniker { get; set; }

        [Required]
        public string Description { get; set; }

        public class Handler : IRequestHandler<UpdateLocationCommand, LocationDto>
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

            public async Task<LocationDto> Handle(UpdateLocationCommand request, CancellationToken cancellationToken)
            {
                await using var dbContext = dbContextFactory.CreateDbContext();

                var location = await dbContext.Locations
                    .FirstOrDefaultAsync(l => l.Moniker == request.Moniker)
                    .ThrowIfNullAsync(problemDetailsFactory.EntityNotFound(nameof(Location), request.Moniker));

                location.Description = request.Description;
                await dbContext.SaveChangesAsync();

                return LocationDto.From(location);
            }
        }

        public class Validator : AbstractValidator<UpdateLocationCommand>
        {
            public Validator()
            {
                RuleFor(r => r.Moniker).NotEmpty();
                RuleFor(r => r.Description).NotEmpty();
            }
        }
    }
}