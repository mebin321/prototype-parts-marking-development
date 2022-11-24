namespace WebApi.Features.Parts.Requests
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Common.Paging;
    using Data;
    using MediatR;
    using Microsoft.EntityFrameworkCore;
    using Models;
    using Swashbuckle.AspNetCore.Annotations;
    using Utilities;

    public class ListPartsQuery : IRequest<PagedDataDto<PartDto>>
    {
        private const int MaxPageSize = 20;

        private int page = 1;
        private int pageSize = 10;

        [Range(1, double.PositiveInfinity)]
        [SwaggerParameter("Page to retrieve.", Required = false)]
        public int Page
        {
            get => page;
            set => page = Math.Max(1, value);
        }

        [Range(1, MaxPageSize)]
        [SwaggerParameter("Page size.", Required = false)]
        public int PageSize
        {
            get => pageSize;
            set => pageSize = Math.Clamp(value, 1, MaxPageSize);
        }

        [SwaggerParameter("Search condition.", Required = false)]
        public string Search { get; set; }

        public class Handler : IRequestHandler<ListPartsQuery, PagedDataDto<PartDto>>
        {
            private readonly IDbContextFactory<PrototypePartsDbContext> dbContextFactory;

            public Handler(IDbContextFactory<PrototypePartsDbContext> dbContextFactory)
            {
                Guard.NotNull(dbContextFactory, nameof(dbContextFactory));

                this.dbContextFactory = dbContextFactory;
            }

            public async Task<PagedDataDto<PartDto>> Handle(ListPartsQuery request, CancellationToken cancellationToken)
            {
                await using var dbContext = dbContextFactory.CreateDbContext();

                var query = dbContext.Parts
                    .AsNoTracking()
                    .OrderBy(l => l.Id)
                    .AsQueryable();

                if (!string.IsNullOrWhiteSpace(request.Search))
                {
                    query = query.Where(l =>
                        EF.Functions.ILike(l.Title, $"%{request.Search}%") ||
                        EF.Functions.ILike(l.Description, $"%{request.Search}%"));
                }

                var parts = await PagedList.CreateAsync(query, request.Page, request.PageSize);

                return new PagedDataDto<PartDto>
                {
                    Items = parts.Select(l => PartDto.From(l)).ToList(),
                    Pagination = parts.CreatePagination(),
                };
            }
        }
    }
}