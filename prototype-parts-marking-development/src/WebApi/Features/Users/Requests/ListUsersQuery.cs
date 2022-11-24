namespace WebApi.Features.Users.Requests
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

    public class ListUsersQuery : IRequest<PagedDataDto<EnrichedUserDto>>
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

        public bool? IsActive { get; set; }

        public class Handler : IRequestHandler<ListUsersQuery, PagedDataDto<EnrichedUserDto>>
        {
            private readonly IDbContextFactory<PrototypePartsDbContext> dbContextFactory;

            public Handler(IDbContextFactory<PrototypePartsDbContext> dbContextFactory)
            {
                Guard.NotNull(dbContextFactory, nameof(dbContextFactory));

                this.dbContextFactory = dbContextFactory;
            }

            public async Task<PagedDataDto<EnrichedUserDto>> Handle(ListUsersQuery request, CancellationToken cancellationToken)
            {
                await using var dbContext = dbContextFactory.CreateDbContext();

                var query = dbContext.Users
                    .AsNoTracking()
                    .OrderBy(u => u.Id)
                    .AsQueryable();

                if (!string.IsNullOrWhiteSpace(request.Search))
                {
                    query = query.Where(u =>
                        EF.Functions.ILike(u.DomainIdentity, $"%{request.Search}%") ||
                        EF.Functions.ILike(u.Email, $"%{request.Search}%"));
                }

                query = request.IsActive switch
                {
                    true => query.Where(u => u.DeletedAt == null),
                    false => query.Where(u => u.DeletedAt != null),
                    _ => query,
                };

                var users = await PagedList.CreateAsync(query, request.Page, request.PageSize);

                var result = new PagedDataDto<EnrichedUserDto>
                {
                    Items = users.Select(EnrichedUserDto.From).ToList(),
                    Pagination = users.CreatePagination(),
                };

                return result;
            }
        }
    }
}