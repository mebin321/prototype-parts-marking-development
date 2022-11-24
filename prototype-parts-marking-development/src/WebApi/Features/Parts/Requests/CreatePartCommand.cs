namespace WebApi.Features.Parts.Requests
{
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Common;
    using Data;
    using FluentValidation;
    using MediatR;
    using Microsoft.EntityFrameworkCore;
    using Models;
    using Utilities;

    public class CreatePartCommand : IRequest<PartDto>
    {
        [Required]
        public string Title { get; set; }

        [Required]
        [MinLength(2)]
        [MaxLength(2)]
        public string Code { get; set; }

        [Required]
        public string Description { get; set; }

        public class Handler : IRequestHandler<CreatePartCommand, PartDto>
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

            public async Task<PartDto> Handle(CreatePartCommand request, CancellationToken cancellationToken)
            {
                await using var dbContext = dbContextFactory.CreateDbContext();

                var moniker = monikerFormatter.Format(request.Title);

                var existing = dbContext.Parts
                    .AsNoTracking()
                    .Count(l => l.Moniker == moniker);

                if (existing != 0)
                {
                    throw new BadRequestException(problemDetailsFactory.BadRequest(
                        "Invalid Part.",
                        "Provided Part already exists. Parts must be unique."));
                }

                var part = new Part
                {
                    Moniker = moniker,
                    Title = request.Title,
                    Code = request.Code,
                    Description = request.Description,
                };
                dbContext.Parts.Add(part);
                await dbContext.SaveChangesAsync();

                return PartDto.From(part);
            }
        }

        public class Validator : AbstractValidator<CreatePartCommand>
        {
            public Validator()
            {
                RuleFor(r => r.Title).NotEmpty();

                RuleFor(r => r.Code)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty()
                    .Must(Validations.NotContainWhitespace).WithMessage("Whitespace characters are not allowed.")
                    .Length(2).WithMessage("Length of the Part Code must be exactly 2 characters.");

                RuleFor(r => r.Description).NotEmpty();
            }
        }
    }
}