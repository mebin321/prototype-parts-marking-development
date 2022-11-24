namespace WebApi.Features.PrintingLabels.Requests
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

    public class ListPrintingLabelsQuery : IRequest<PagedDataDto<PrintingLabelDto>>
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

        public int? OwnerId { get; set; }

        public class Handler : IRequestHandler<ListPrintingLabelsQuery, PagedDataDto<PrintingLabelDto>>
        {
            private readonly IDbContextFactory<PrototypePartsDbContext> dbContextFactory;

            public Handler(IDbContextFactory<PrototypePartsDbContext> dbContextFactory)
            {
                Guard.NotNull(dbContextFactory, nameof(dbContextFactory));

                this.dbContextFactory = dbContextFactory;
            }

            public async Task<PagedDataDto<PrintingLabelDto>> Handle(ListPrintingLabelsQuery request, CancellationToken cancellationToken)
            {
                await using var dbContext = dbContextFactory.CreateDbContext();

                var query = dbContext.PrintingLabels
                    .AsNoTracking()
                    .OrderBy(l => l.Id)
                    .AsQueryable();

                if (request.OwnerId is not null)
                {
                    query = query.Where(l => l.OwnerId == request.OwnerId);
                }

                var printingLabels = await PagedList.CreateAsync(query, request.Page, request.PageSize);

                return new PagedDataDto<PrintingLabelDto>
                {
                    Items = printingLabels.Select(p => PrintingLabelDto.From(p)).ToList(),
                    Pagination = printingLabels.CreatePagination(),
                };
            }
        }
    }
}
