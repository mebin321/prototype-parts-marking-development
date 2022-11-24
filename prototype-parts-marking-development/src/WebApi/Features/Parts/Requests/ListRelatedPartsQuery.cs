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

    public class ListRelatedPartsQuery : IRequest<PartDto[]>
    {
        [FromRoute(Name = "moniker")]
        public string Moniker { get; set; }

        [FromQuery(Name = "Search")]
        public string Search { get; set; }

        public class Handler : IRequestHandler<ListRelatedPartsQuery, PartDto[]>
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

            public async Task<PartDto[]> Handle(ListRelatedPartsQuery request, CancellationToken cancellationToken)
            {
                await using var dbContext = dbContextFactory.CreateDbContext();

                var outlet = await dbContext.ProductGroups
                    .AsNoTracking()
                    .Include(e => e.ProductGroupToPartRelations
                        .Where(r =>
                            string.IsNullOrEmpty(request.Search)
                            || EF.Functions.ILike(r.Part.Title, $"%{request.Search}%")
                            || EF.Functions.ILike(r.Part.Description, $"%{request.Search}%")))
                    .ThenInclude(e => e.Part)
                    .FirstOrDefaultAsync(e => e.Moniker == request.Moniker, CancellationToken.None)
                    .ThrowIfNullAsync(problemDetailsFactory.EntityNotFound(nameof(ProductGroup), request.Moniker));

                return outlet.ProductGroupToPartRelations.Select(r => PartDto.From(r.Part)).ToArray();
            }
        }

        public class Validator : AbstractValidator<ListRelatedPartsQuery>
        {
            public Validator()
            {
                RuleFor(r => r.Moniker).NotEmpty();
            }
        }
    }
}