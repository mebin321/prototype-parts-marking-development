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

    public class RemoveUserRoleCommand : IRequest<List<RoleDto>>
    {
        public int UserId { get; set; }

        public string RoleMoniker { get; set; }

        public class Handler : IRequestHandler<RemoveUserRoleCommand, List<RoleDto>>
        {
            private readonly IDbContextFactory<PrototypePartsDbContext> dbContextFactory;
            private readonly IProblemDetailsFactory problemDetailsFactory;
            private readonly IDateTime dateTime;

            public Handler(IDbContextFactory<PrototypePartsDbContext> dbContextFactory, IProblemDetailsFactory problemDetailsFactory, IDateTime dateTime)
            {
                Guard.NotNull(dbContextFactory, nameof(dbContextFactory));
                Guard.NotNull(problemDetailsFactory, nameof(problemDetailsFactory));
                Guard.NotNull(dateTime, nameof(dateTime));

                this.dbContextFactory = dbContextFactory;
                this.problemDetailsFactory = problemDetailsFactory;
                this.dateTime = dateTime;
            }

            public async Task<List<RoleDto>> Handle(RemoveUserRoleCommand request, CancellationToken cancellationToken)
            {
                await using var dbContext = dbContextFactory.CreateDbContext();

                var user = await dbContext.Users
                    .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                    .FirstOrDefaultAsync(u => u.Id == request.UserId, CancellationToken.None)
                    .ThrowIfNullAsync(problemDetailsFactory.EntityNotFound(nameof(User), request.UserId));

                if (user.UserRoles.RemoveAll(ur => ur.Role.Moniker == request.RoleMoniker) != 0)
                {
                    user.ModifiedAt = dateTime.Now;

                    await dbContext.SaveChangesAsync();
                }

                return user.UserRoles.Select(ur => RoleDto.From(ur.Role)).ToList();
            }
        }
    }
}