namespace WebApi.Features.Users.Requests
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using FluentValidation;
    using MediatR;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Utilities;
    using WebApi.Common.Paging;
    using WebApi.Data;
    using WebApi.Features.Users.Models;

    public class ListRoleUsersQuery : IRequest<PagedDataDto<UserDto>>
    {
        private const int MaxPageSize = 20;

        private int page = 1;
        private int pageSize = 10;

        [FromRoute(Name = "moniker")]
        public string Moniker { get; set; }

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

        public string Search { get; set; }

        public bool? IsActive { get; set; }

        public class Handler : IRequestHandler<ListRoleUsersQuery, PagedDataDto<UserDto>>
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

            public async Task<PagedDataDto<UserDto>> Handle(ListRoleUsersQuery request, CancellationToken cancellationToken)
            {
                await using var dbContext = dbContextFactory.CreateDbContext();

                var role = await dbContext.Roles
                    .AsNoTracking()
                    .FirstOrDefaultAsync(r => r.Moniker == request.Moniker, CancellationToken.None);

                if (role is null)
                {
                    throw new NotFoundException(problemDetailsFactory.NotFound(
                        "Role not found.",
                        $"Could not find Role with ID {request.Moniker}."));
                }

                var query = dbContext.UserRoles
                    .AsNoTracking()
                    .Where(ur => ur.RoleId == role.Id)
                    .Select(ur => ur.User);

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

                query = query.OrderBy(u => u.Id);

                var users = await PagedList.CreateAsync(query, request.Page, request.PageSize);

                return new PagedDataDto<UserDto>
                {
                    Items = users.Select(UserDto.From).ToList(),
                    Pagination = users.CreatePagination(),
                };
            }
        }

        public class Validator : AbstractValidator<ListRoleUsersQuery>
        {
            public Validator()
            {
                RuleFor(r => r.Moniker).NotEmpty();
            }
        }
    }
}