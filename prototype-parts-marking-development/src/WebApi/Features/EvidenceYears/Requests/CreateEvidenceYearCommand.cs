namespace WebApi.Features.EvidenceYears.Requests
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

    public class CreateEvidenceYearCommand : IRequest<EvidenceYearDto>
    {
        [Required]
        public int Year { get; set; }

        [Required]
        [MinLength(2)]
        [MaxLength(2)]
        public string Code { get; set; }

        public class Handler : IRequestHandler<CreateEvidenceYearCommand, EvidenceYearDto>
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

            public async Task<EvidenceYearDto> Handle(
                CreateEvidenceYearCommand request,
                CancellationToken cancellationToken)
            {
                await using var dbContext = dbContextFactory.CreateDbContext();

                var existing = dbContext.EvidenceYears
                    .AsNoTracking()
                    .Count(e => e.Year == request.Year);

                if (existing != 0)
                {
                    throw new BadRequestException(problemDetailsFactory.BadRequest(
                        "Invalid EvidenceYear.",
                        "Provided EvidenceYear already exists. EvidenceYears must be unique."));
                }

                var evidenceYear = new EvidenceYear
                {
                    Year = request.Year,
                    Code = request.Code,
                };
                dbContext.EvidenceYears.Add(evidenceYear);
                await dbContext.SaveChangesAsync();

                return EvidenceYearDto.From(evidenceYear);
            }
        }

        public class Validator : AbstractValidator<CreateEvidenceYearCommand>
        {
            public Validator()
            {
                RuleFor(r => r.Code)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty()
                    .Must(Validations.NotContainWhitespace).WithMessage("Whitespace characters are not allowed.")
                    .Length(2).WithMessage("Length of the EvidenceYear Code must be exactly 2 characters.");
            }
        }
    }
}