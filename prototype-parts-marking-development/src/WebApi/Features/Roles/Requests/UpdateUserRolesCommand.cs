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

    public class UpdateUserRolesCommand : IRequest<List<RoleDto>>
    {
        public int UserId { get; set; }

        public List<string> RoleMonikers { get; set; }

        public class Handler : IRequestHandler<UpdateUserRolesCommand, List<RoleDto>>
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

            public async Task<List<RoleDto>> Handle(UpdateUserRolesCommand request, CancellationToken cancellationToken)
            {
                await using var dbContext = dbContextFactory.CreateDbContext();

                var user = await dbContext.Users
                    .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                    .FirstOrDefaultAsync(u => u.Id == request.UserId, CancellationToken.None)
                    .ThrowIfNullAsync(problemDetailsFactory.EntityNotFound(nameof(User), request.UserId));

                var monikers = request.RoleMonikers.Distinct().ToList();

                if (monikers.Count == 0)
                {
                    user.UserRoles.Clear();
                }
                else
                {
                    var roles = await dbContext.Roles
                        .Where(r => monikers.Contains(r.Moniker))
                        .ToListAsync(CancellationToken.None);

                    if (monikers.Count != roles.Count)
                    {
                        throw new BadRequestException(problemDetailsFactory.BadRequest(
                            "Invalid role.",
                            "Request contains one or more invalid roles."));
                    }

                    user.UserRoles.Clear();
                    user.UserRoles.AddRange(roles.Select(r => new UserRole
                    {
                        UserId = user.Id,
                        RoleId = r.Id,
                    }));
                }

                user.ModifiedAt = dateTime.Now;
                await dbContext.SaveChangesAsync();

                return user.UserRoles.Select(ur => RoleDto.From(ur.Role)).ToList();
            }
        }
    }
}