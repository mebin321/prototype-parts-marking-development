namespace WebApi.Features.EvidenceYears.Requests
{
    using System.Threading;
    using System.Threading.Tasks;
    using Data;
    using FluentValidation;
    using MediatR;
    using Microsoft.EntityFrameworkCore;
    using Models;
    using Utilities;

    public class GetEvidenceYearQuery : IRequest<EvidenceYearDto>
    {
        public int Year { get; set; }

        public class Handler : IRequestHandler<GetEvidenceYearQuery, EvidenceYearDto>
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

            public async Task<EvidenceYearDto> Handle(GetEvidenceYearQuery request, CancellationToken cancellationToken)
            {
                await using var dbContext = dbContextFactory.CreateDbContext();

                var evidenceYear = await dbContext.EvidenceYears
                    .AsNoTracking()
                    .FirstOrDefaultAsync(y => y.Year == request.Year);

                if (evidenceYear is null)
                {
                    throw new NotFoundException(problemDetailsFactory.NotFound(
                        "EvidenceYear not found.",
                        $"Could not find EvidenceYear with ID {request.Year}."));
                }

                return EvidenceYearDto.From(evidenceYear);
            }
        }

        public class Validator : AbstractValidator<GetEvidenceYearQuery>
        {
            public Validator()
            {
                RuleFor(r => r.Year).NotEmpty();
            }
        }
    }
}
