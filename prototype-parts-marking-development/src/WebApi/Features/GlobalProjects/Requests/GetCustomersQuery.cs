namespace WebApi.Features.GlobalProjects.Requests
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using Microsoft.EntityFrameworkCore;
    using Utilities;
    using WebApi.Data;

    public class GetCustomersQuery : IRequest<string[]>
    {
        public class Handler : IRequestHandler<GetCustomersQuery, string[]>
        {
            private readonly IDbContextFactory<PrototypePartsDbContext> dbContextFactory;

            public Handler(IDbContextFactory<PrototypePartsDbContext> dbContextFactory)
            {
                Guard.NotNull(dbContextFactory, nameof(dbContextFactory));

                this.dbContextFactory = dbContextFactory;
            }

            public async Task<string[]> Handle(GetCustomersQuery request, CancellationToken cancellationToken)
            {
                await using var context = dbContextFactory.CreateDbContext();

                return await context.GlobalProjects
                    .AsNoTracking()
                    .Select(p => p.Customer)
                    .Distinct()
                    .ToArrayAsync(CancellationToken.None);
            }
        }
    }
}
