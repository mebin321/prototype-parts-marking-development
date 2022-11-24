namespace WebApi.Features.Roles.Requests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using Microsoft.EntityFrameworkCore;
    using Utilities;
    using WebApi.Common;
    using WebApi.Data;
    using WebApi.Features.Roles.Models;

    public class AddUserRoleCommand : IRequest<List<RoleDto>>
    {
        public int UserId { get; set; }

        public string RoleMoniker { get; set; }

        public class Handler : IRequestHandler<AddUserRoleCommand, List<RoleDto>>
        {
            private readonly IDbContextFactory<PrototypePartsDbContext> dbContextFactory;
            private readonly IProblemDetailsFactory problemDetailsFactory;
            private readonly IDateTime dateTime;

            public Handler(
                IDbContextFactory<PrototypePartsDbContext> dbContextFactory,
                IProblemDetailsFactory problemDetailsFactory,
                IDateTime dateTime)
            {
                Guard.NotNull(dbContextFactory, nameof(dbContextFactory));
                Guard.NotNull(problemDetailsFactory, nameof(problemDetailsFactory));
                Guard.NotNull(dateTime, nameof(dateTime));

                this.dbContextFactory = dbContextFactory;
                this.problemDetailsFactory = problemDetailsFactory;
                this.dateTime = dateTime;
            }

            public async Task<List<RoleDto>> Handle(AddUserRoleCommand request, CancellationToken cancellationToken)
            {
                await using var dbContext = dbContextFactory.CreateDbContext();

                var user = await dbContext.Users
                    .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                    .FirstOrDefaultAsync(u => u.Id == request.UserId, CancellationToken.None)
                    .ThrowIfNullAsync(problemDetailsFactory.EntityNotFound(nameof(User), request.UserId));

                var role = await dbContext.Roles
                    .FirstOrDefaultAsync(r => r.Moniker == request.RoleMoniker, CancellationToken.None)
                    .ThrowIfNullAsync(problemDetailsFactory.EntityNotFound(nameof(Role), request.RoleMoniker));

                if (user.UserRoles.Count(ur => ur.RoleId == role.Id) == 0)
                {
                    user.UserRoles.Add(new UserRole
                    {
                        UserId = user.Id,
                        RoleId = role.Id,
                    });
                    user.ModifiedAt = dateTime.Now;

                    await dbContext.SaveChangesAsync();
                }

                return user.UserRoles.Select(ur => RoleDto.From(ur.Role)).ToList();
            }
        }
    }
}