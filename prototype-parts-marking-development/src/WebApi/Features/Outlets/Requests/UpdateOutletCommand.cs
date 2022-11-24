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

    public class UpdateOutletCommand : IRequest<OutletDto>
    {
        public string Moniker { get; set; }

        public string Description { get; set; }

        public class Handler : IRequestHandler<UpdateOutletCommand, OutletDto>
        {
            private readonly IDbContextFactory<PrototypePartsDbContext> dbContextFactory;
            private readonly IProblemDetailsFactory problemDetailsFactory;

            public Handler(
                IDbContextFactory<PrototypePartsDbContext> dbContextFactory,
                IProblemDetailsFactory problemDetailsFactory)
            {
                Guard.NotNull(dbContextFactory, nameof(dbContextFactory));
                Guard.NotNull(problemDetailsFactory, nameof(problemDetailsFactory));

                this.dbContextFactory = dbContextFactory;
                this.problemDetailsFactory = problemDetailsFactory;
            }

            public async Task<OutletDto> Handle(UpdateOutletCommand request, CancellationToken cancellationToken)
            {
                await using var dbContext = dbContextFactory.CreateDbContext();

                var outlet = await dbContext.Outlets
                    .FirstOrDefaultAsync(l => l.Moniker == request.Moniker);

                if (outlet is null)
                {
                    throw new NotFoundException(problemDetailsFactory.NotFound(
                        "Outlet not found.",
                        $"Could not find Outlet with ID {request.Moniker}."));
                }

                outlet.Description = request.Description;
                await dbContext.SaveChangesAsync();

                return OutletDto.From(outlet);
            }
        }

        public class Validator : AbstractValidator<UpdateOutletCommand>
        {
            public Validator()
            {
                RuleFor(r => r.Moniker).NotEmpty();
                RuleFor(r => r.Description).NotEmpty();
            }
        }
    }
}
