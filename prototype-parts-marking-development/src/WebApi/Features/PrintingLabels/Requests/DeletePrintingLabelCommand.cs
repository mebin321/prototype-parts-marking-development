namespace WebApi.Features.PrintingLabels.Requests
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Authentication.Services;
    using Authorization;
    using Data;
    using FluentValidation;
    using MediatR;
    using Microsoft.EntityFrameworkCore;
    using Utilities;

    public class DeletePrintingLabelCommand : IRequest
    {
        public int LabelId { get; set; }

        public class Handler : AsyncRequestHandler<DeletePrintingLabelCommand>
        {
            private readonly IDbContextFactory<PrototypePartsDbContext> dbContextFactory;
            private readonly IProblemDetailsFactory problemDetailsFactory;
            private readonly ICurrentUserAccessor currentUserAccessor;

            public Handler(IDbContextFactory<PrototypePartsDbContext> dbContextFactory, IProblemDetailsFactory problemDetailsFactory, ICurrentUserAccessor currentUserAccessor)
            {
                Guard.NotNull(dbContextFactory, nameof(dbContextFactory));
                Guard.NotNull(problemDetailsFactory, nameof(problemDetailsFactory));
                Guard.NotNull(currentUserAccessor, nameof(currentUserAccessor));

                this.dbContextFactory = dbContextFactory;
                this.problemDetailsFactory = problemDetailsFactory;
                this.currentUserAccessor = currentUserAccessor;
            }

            protected override async Task Handle(DeletePrintingLabelCommand request, CancellationToken cancellationToken)
            {
                using var dbContext = dbContextFactory.CreateDbContext();

                var printingLabel = await dbContext.PrintingLabels
                    .FirstOrDefaultAsync(u => u.Id == request.LabelId)
                    .ThrowIfNullAsync(problemDetailsFactory.EntityNotFound(nameof(PrintingLabel), request.LabelId));

                var user = currentUserAccessor.GetCurrentUser();
                var userRoles = dbContext.UserRoles
                    .Include(r => r.Role)
                    .Where(r => r.UserId == user)
                    .Select(r => r.Role.Moniker)
                    .ToList();

                if (printingLabel.OwnerId == user
                    || userRoles.Any(r => r == RoleName.Admin || r == RoleName.SuperAdmin))
                {
                    dbContext.PrintingLabels.Remove(printingLabel);
                    await dbContext.SaveChangesAsync(cancellationToken);
                }
                else
                {
                    throw new NotAuthorizedException(problemDetailsFactory.Unauthorized(
                        "Unauthorized.",
                        $"User with ID {user} cannot delete printing label with ID {request.LabelId}."));
                }
            }
        }

        public class Validator : AbstractValidator<DeletePrintingLabelCommand>
        {
            public Validator()
            {
                RuleFor(r => r.LabelId).GreaterThan(0);
            }
        }
    }
}