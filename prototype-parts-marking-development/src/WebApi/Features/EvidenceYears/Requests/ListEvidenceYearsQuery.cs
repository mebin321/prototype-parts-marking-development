namespace WebApi.Features.EvidenceYears.Requests
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
    using Utilities;

    public class ListEvidenceYearsQuery : IRequest<PagedDataDto<EvidenceYearDto>>
    {
        private const int MaxPageSize = 20;

        private int page = 1;
        private int pageSize = 10;

        [Range(1, double.PositiveInfinity)]
        public int Page
        {
            get => page;
            set => page = Math.Max(1, value);
        }

        [Range(1, MaxPageSize)]
        public int PageSize
        {
            get => pageSize;
            set => pageSize = Math.Clamp(value, 1, MaxPageSize);
        }

        public class Handler : IRequestHandler<ListEvidenceYearsQuery, PagedDataDto<EvidenceYearDto>>
        {
            private readonly IDbContextFactory<PrototypePartsDbContext> dbContextFactory;

            public Handler(IDbContextFactory<PrototypePartsDbContext> dbContextFactory)
            {
                Guard.NotNull(dbContextFactory, nameof(dbContextFactory));

                this.dbContextFactory = dbContextFactory;
            }

            public async Task<PagedDataDto<EvidenceYearDto>> Handle(ListEvidenceYearsQuery request, CancellationToken cancellationToken)
            {
                await using var dbContext = dbContextFactory.CreateDbContext();

                var query = dbContext.EvidenceYears
                    .AsNoTracking()
                    .OrderBy(y => y.Id)
                    .AsQueryable();

                var yearOfEvidences = await PagedList.CreateAsync(query, request.Page, request.PageSize);

                return new PagedDataDto<EvidenceYearDto>
                {
                    Items = yearOfEvidences.Select(y => EvidenceYearDto.From(y)).ToList(),
                    Pagination = yearOfEvidences.CreatePagination(),
                };
            }
        }
    }
}