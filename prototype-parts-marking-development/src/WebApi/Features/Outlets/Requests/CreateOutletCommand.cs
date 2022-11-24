namespace WebApi.Features.Outlets.Requests
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

    public class CreateOutletCommand : IRequest<OutletDto>
    {
        [Required]
        public string Title { get; set; }

        [Required]
        [MinLength(2)]
        [MaxLength(2)]
        public string Code { get; set; }

        [Required]
        public string Description { get; set; }

        public class Handler : IRequestHandler<CreateOutletCommand, OutletDto>
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

            public async Task<OutletDto> Handle(CreateOutletCommand request, CancellationToken cancellationToken)
            {
                await using var dbContext = dbContextFactory.CreateDbContext();

                var moniker = monikerFormatter.Format(request.Title);

                var existing = dbContext.Outlets
                    .AsNoTracking()
                    .Count(l => l.Moniker == moniker);

                if (existing != 0)
                {
                    throw new BadRequestException(problemDetailsFactory.BadRequest(
                        "Invalid Outlet.",
                        "Provided Outlet already exists. Outlets must be unique."));
                }

                var outlet = new Outlet
                {
                    Moniker = moniker,
                    Title = request.Title,
                    Code = request.Code,
                    Description = request.Description,
                };
                dbContext.Outlets.Add(outlet);
                await dbContext.SaveChangesAsync();

                return OutletDto.From(outlet);
            }
        }

        public class Validator : AbstractValidator<CreateOutletCommand>
        {
            public Validator()
            {
                RuleFor(r => r.Title).NotEmpty();

                RuleFor(r => r.Code)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty()
                    .Must(Validations.BeNumeric).WithMessage("Outlet Code must be numeric.")
                    .Length(2).WithMessage("Length of the Outlet Code must be exactly 2 characters.");

                RuleFor(r => r.Description).NotEmpty();
            }
        }
    }
}
