namespace WebApi.Features.ProductGroup.Requests
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
    using WebApi.Features.ProductGroup.Models;

    public class ListRelatedProductGroupsQuery : IRequest<ProductGroupDto[]>
    {
        [FromRoute(Name = "moniker")]
        public string Moniker { get; set; }

        [FromQuery(Name = "Search")]
        public string Search { get; set; }

        public class Handler : IRequestHandler<ListRelatedProductGroupsQuery, ProductGroupDto[]>
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

            public async Task<ProductGroupDto[]> Handle(ListRelatedProductGroupsQuery request, CancellationToken cancellationToken)
            {
                await using var dbContext = dbContextFactory.CreateDbContext();

                var outlet = await dbContext.Outlets
                    .AsNoTracking()
                    .Include(e => e.OutletToProductGroupRelations
                    .Where(r =>
                        string.IsNullOrEmpty(request.Search)
                        || EF.Functions.ILike(r.ProductGroup.Title, $"%{request.Search}%")
                        || EF.Functions.ILike(r.ProductGroup.Description, $"%{request.Search}%")))
                    .ThenInclude(e => e.ProductGroup)
                    .FirstOrDefaultAsync(e => e.Moniker == request.Moniker, CancellationToken.None)
                    .ThrowIfNullAsync(problemDetailsFactory.EntityNotFound(nameof(Outlet), request.Moniker));

                return outlet.OutletToProductGroupRelations.Select(r => ProductGroupDto.From(r.ProductGroup)).ToArray();
            }
        }

        public class Validator : AbstractValidator<ListRelatedProductGroupsQuery>
        {
            public Validator()
            {
                RuleFor(r => r.Moniker).NotEmpty();
            }
        }
    }
}