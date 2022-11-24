namespace WebApi.Features.GateLevels.Requests
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Common.Paging;
    using Data;
    using MediatR;
    using Microsoft.EntityFrameworkCore;
    using Models;
    using Utilities;

    public class ListGateLevelsQuery : IRequest<PagedDataDto<GateLevelDto>>
    {
        private const int MaxPageSize = 20;

        private int page = 1;
        private int pageSize = 10;

        public int Page
        {
            get => page;
            set => page = Math.Max(1, value);
        }

        public int PageSize
        {
            get => pageSize;
            set => pageSize = Math.Clamp(value, 1, MaxPageSize);
        }

        public string Search { get; set; }

        public class Handler : IRequestHandler<ListGateLevelsQuery, PagedDataDto<GateLevelDto>>
        {
            private readonly IDbContextFactory<PrototypePartsDbContext> dbContextFactory;

            public Handler(IDbContextFactory<PrototypePartsDbContext> dbContextFactory)
            {
                Guard.NotNull(dbContextFactory, nameof(dbContextFactory));

                this.dbContextFactory = dbContextFactory;
            }

            public async Task<PagedDataDto<GateLevelDto>> Handle(ListGateLevelsQuery request, CancellationToken cancellationToken)
            {
                await using var dbContext = dbContextFactory.CreateDbContext();

                var query = dbContext.GateLevels
                    .AsNoTracking()
                    .OrderBy(l => l.Id)
                    .AsQueryable();

                if (!string.IsNullOrWhiteSpace(request.Search))
                {
                    query = query.Where(l =>
                        EF.Functions.ILike(l.Title, $"%{request.Search}%") ||
                        EF.Functions.ILike(l.Description, $"%{request.Search}%"));
                }

                var gateLevels = await PagedList.CreateAsync(query, request.Page, request.PageSize);

                return new PagedDataDto<GateLevelDto>
                {
                    Items = gateLevels.Select(l => GateLevelDto.From(l)).ToList(),
                    Pagination = gateLevels.CreatePagination(),
                };
            }
        }
    }
}