namespace WebApi.Features.Roles.Requests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Utilities;
    using WebApi.Data;
    using WebApi.Features.Roles.Models;

    public class ListUserRolesQuery : IRequest<List<RoleDto>>
    {
        [FromRoute(Name = "userId")]
        public int UserId { get; set; }

        public class Handler : IRequestHandler<ListUserRolesQuery, List<RoleDto>>
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

            public async Task<List<RoleDto>> Handle(ListUserRolesQuery request, CancellationToken cancellationToken)
            {
                await using var dbContext = dbContextFactory.CreateDbContext();

                var user = await dbContext.Users
                    .AsNoTracking()
                    .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                    .FirstOrDefaultAsync(u => u.Id == request.UserId, CancellationToken.None)
                    .ThrowIfNullAsync(problemDetailsFactory.EntityNotFound(nameof(User), request.UserId));

                return user.UserRoles.Select(ur => RoleDto.From(ur.Role)).ToList();
            }
        }
    }
}