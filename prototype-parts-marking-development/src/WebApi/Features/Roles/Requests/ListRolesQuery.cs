namespace WebApi.Features.Roles.Requests
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using Microsoft.EntityFrameworkCore;
    using Swashbuckle.AspNetCore.Annotations;
    using Utilities;
    using WebApi.Common.Paging;
    using WebApi.Data;
    using WebApi.Features.Roles.Models;

    public class ListRolesQuery : IRequest<PagedDataDto<RoleDto>>
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

        public class Handler : IRequestHandler<ListRolesQuery, PagedDataDto<RoleDto>>
        {
            private readonly IDbContextFactory<PrototypePartsDbContext> dbContextFactory;

            public Handler(IDbContextFactory<PrototypePartsDbContext> dbContextFactory)
            {
                Guard.NotNull(dbContextFactory, nameof(dbContextFactory));

                this.dbContextFactory = dbContextFactory;
            }

            public async Task<PagedDataDto<RoleDto>> Handle(ListRolesQuery request, CancellationToken cancellationToken)
            {
                await using var dbContext = dbContextFactory.CreateDbContext();

                var query = dbContext.Roles
                    .AsNoTracking()
                    .OrderBy(l => l.Id)
                    .AsQueryable();

                if (!string.IsNullOrWhiteSpace(request.Search))
                {
                    query = query.Where(l =>
                        EF.Functions.ILike(l.Title, $"%{request.Search}%") ||
                        EF.Functions.ILike(l.Description, $"%{request.Search}%"));
                }

                var roles = await PagedList.CreateAsync(query, request.Page, request.PageSize);

                return new PagedDataDto<RoleDto>
                {
                    Items = roles.Select(RoleDto.From).ToList(),
                    Pagination = roles.CreatePagination(),
                };
            }
        }
    }
}