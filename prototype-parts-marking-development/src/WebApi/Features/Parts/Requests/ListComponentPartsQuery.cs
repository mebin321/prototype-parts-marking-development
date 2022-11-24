namespace WebApi.Features.Parts.Requests
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using FluentValidation;
    using MediatR;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Utilities;
    using WebApi.Data;
    using WebApi.Features.Parts.Models;

    public class ListComponentPartsQuery : IRequest<PartDto[]>
    {
        [FromRoute(Name = "moniker")]
        public string Moniker { get; set; }

        [FromQuery(Name = "Search")]
        public string Search { get; set; }

        public class Handler : IRequestHandler<ListComponentPartsQuery, PartDto[]>
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

            public async Task<PartDto[]> Handle(ListComponentPartsQuery request, CancellationToken cancellationToken)
            {
                await using var dbContext = dbContextFactory.CreateDbContext();

                var part = await dbContext.Parts
                    .Include(e => e.PartToComponentRelations
                        .Where(r =>
                            string.IsNullOrEmpty(request.Search)
                            || EF.Functions.ILike(r.ComponentPart.Title, $"%{request.Search}%")
                            || EF.Functions.ILike(r.ComponentPart.Description, $"%{request.Search}%")))
                    .ThenInclude(e => e.ComponentPart)
                    .FirstOrDefaultAsync(e => e.Moniker == request.Moniker, CancellationToken.None)
                    .ThrowIfNullAsync(problemDetailsFactory.EntityNotFound(nameof(Part), request.Moniker));

                return part.PartToComponentRelations.Select(r => PartDto.From(r.ComponentPart)).ToArray();
            }
        }

        public class Validator : AbstractValidator<ListComponentPartsQuery>
        {
            public Validator()
            {
                RuleFor(r => r.Moniker).NotEmpty();
            }
        }
    }
}