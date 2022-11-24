namespace WebApi.Features.PrototypeVariants.Requests
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

    public class FilteredListPrototypeVariantsQuery : IRequest<PagedDataDto<EnrichPrototypeVariantDto>>
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

        [SwaggerParameter("String used for fulltext search in comment.", Required = false)]
        public string Search { get; set; }

        [SwaggerParameter("Filtering condition for active prototypes.", Required = false)]
        public bool? IsPrototypeActive { get; set; }

        public class Handler : IRequestHandler<FilteredListPrototypeVariantsQuery, PagedDataDto<EnrichPrototypeVariantDto>>
        {
            private readonly IDbContextFactory<PrototypePartsDbContext> dbContextFactory;

            public Handler(IDbContextFactory<PrototypePartsDbContext> dbContextFactory)
            {
                Guard.NotNull(dbContextFactory, nameof(dbContextFactory));

                this.dbContextFactory = dbContextFactory;
            }

            public async Task<PagedDataDto<EnrichPrototypeVariantDto>> Handle(
                FilteredListPrototypeVariantsQuery request,
                CancellationToken cancellationToken)
            {
                await using var dbContext = dbContextFactory.CreateDbContext();

                var query = dbContext.PrototypeVariants
                    .AsNoTracking()
                    .Include(v => v.Prototype).ThenInclude(p => p.PrototypeSet).ThenInclude(s => s.CreatedBy)
                    .Include(v => v.Prototype).ThenInclude(p => p.PrototypeSet).ThenInclude(s => s.ModifiedBy)
                    .Include(v => v.Prototype).ThenInclude(p => p.PrototypeSet).ThenInclude(s => s.DeletedBy)
                    .Include(v => v.Prototype).ThenInclude(p => p.Owner)
                    .Include(v => v.Prototype).ThenInclude(p => p.CreatedBy)
                    .Include(v => v.Prototype).ThenInclude(p => p.ModifiedBy)
                    .Include(v => v.Prototype).ThenInclude(p => p.DeletedBy)
                    .Include(v => v.CreatedBy)
                    .Include(v => v.ModifiedBy)
                    .Include(v => v.DeletedBy)
                    .OrderBy(v => v.Prototype.Id).ThenByDescending(v => v.Version)
                    .AsQueryable();

                query = request.IsPrototypeActive switch
                {
                    true => query.Where(v => v.Prototype.DeletedAt == null),
                    false => query.Where(v => v.Prototype.DeletedAt != null),
                    _ => query,
                };

                if (!string.IsNullOrWhiteSpace(request.Search))
                {
                    query = query.Where(v => EF.Functions.ILike(v.Comment, $"%{request.Search}%"));
                }

                var variants = await PagedList.CreateAsync(query, request.Page, request.PageSize);

                return new PagedDataDto<EnrichPrototypeVariantDto>
                {
                    Items = variants.Select(p => EnrichPrototypeVariantDto.From(p)).ToList(),
                    Pagination = variants.CreatePagination(),
                };
            }
        }
    }
}