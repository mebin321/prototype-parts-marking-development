namespace WebApi.Features.Locations.Requests
{
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Data;
    using FluentValidation;
    using MediatR;
    using Microsoft.EntityFrameworkCore;
    using Models;
    using Utilities;
    using WebApi.Common;

    public class CreateLocationCommand : IRequest<LocationDto>
    {
        [Required]
        public string Title { get; set; }

        [Required]
        [MinLength(2)]
        [MaxLength(2)]
        public string Code { get; set; }

        [Required]
        public string Description { get; set; }

        public class Handler : IRequestHandler<CreateLocationCommand, LocationDto>
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

            public async Task<LocationDto> Handle(CreateLocationCommand request, CancellationToken cancellationToken)
            {
                await using var dbContext = dbContextFactory.CreateDbContext();

                var moniker = monikerFormatter.Format(request.Title);

                var existing = dbContext.Locations
                    .AsNoTracking()
                    .Count(l => l.Moniker == moniker);

                if (existing != 0)
                {
                    throw new BadRequestException(problemDetailsFactory.BadRequest(
                        "Invalid Location.",
                        "Provided Location already exists. Locations must be unique."));
                }

                var location = new Location
                {
                    Moniker = moniker,
                    Title = request.Title,
                    Code = request.Code,
                    Description = request.Description,
                };
                dbContext.Locations.Add(location);
                await dbContext.SaveChangesAsync();

                return LocationDto.From(location);
            }
        }

        public class Validator : AbstractValidator<CreateLocationCommand>
        {
            public Validator()
            {
                RuleFor(r => r.Title).NotEmpty();

                RuleFor(r => r.Code)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty()
                    .Must(Validations.NotContainWhitespace).WithMessage("Whitespace characters are not allowed.")
                    .Length(2).WithMessage("Length of the Location Code must be exactly 2 characters.");

                RuleFor(r => r.Description).NotEmpty();
            }
        }
    }
}