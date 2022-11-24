namespace WebApi.Features.Prototypes.Requests
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Authentication.Services;
    using Data;
    using FluentValidation;
    using MediatR;
    using Microsoft.EntityFrameworkCore;
    using Utilities;

    public class ReactivatePrototypeCommand : IRequest
    {
        public int SetId { get; set; }

        public int PrototypeId { get; set; }

        public class Handler : AsyncRequestHandler<ReactivatePrototypeCommand>
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

            protected override async Task Handle(ReactivatePrototypeCommand request, CancellationToken cancellationToken)
            {
                await using var dbContext = dbContextFactory.CreateDbContext();

                var set = await dbContext.PrototypeSets
                    .Include(s => s.Prototypes)
                    .FirstOrDefaultAsync(s => s.Id == request.SetId, CancellationToken.None)
                    .ThrowIfNullAsync(problemDetailsFactory.EntityNotFound(nameof(PrototypeSet), request.SetId));

                var prototype = set.Prototypes.FirstOrDefault(p => p.Id == request.PrototypeId);

                if (prototype is null)
                {
                    throw problemDetailsFactory.EntityNotFound(nameof(Prototype), request.PrototypeId);
                }

                if (prototype.DeletedAt is not null)
                {
                    CheckUniqueness(set, prototype);

                    var currentUser = await GetUserAsync(dbContext, currentUserAccessor.GetCurrentUser());

                    prototype.DeletedAt = null;
                    prototype.DeletedById = null;
                    prototype.ModifiedById = currentUser.Id;

                    await dbContext.SaveChangesAsync(CancellationToken.None);
                }
            }

            private void CheckUniqueness(PrototypeSet prototypeSet, Prototype prototype)
            {
                var existingPrototype = prototypeSet.Prototypes.FirstOrDefault(
                    p => p.Index == prototype.Index && p.PartTypeCode == prototype.PartTypeCode && p.DeletedAt == null);

                if (existingPrototype is not null)
                {
                    var details = $"Cannot reactivate Component with Id {prototype.Id}." +
                                  $" Active Component with Index {prototype.Index} and Part Code {prototype.PartTypeCode} already exists.";

                    throw new BadRequestException(problemDetailsFactory.BadRequest("Invalid PrototypeId.", details));
                }
            }

            private async Task<User> GetUserAsync(PrototypePartsDbContext dbContext, int id)
            {
                return await dbContext.Users
                    .SingleOrDefaultAsync(u => u.Id == id, CancellationToken.None)
                    .ThrowIfNullAsync(problemDetailsFactory.EntityNotFound(nameof(User), id));
            }
        }

        public class Validator : AbstractValidator<ReactivatePrototypeCommand>
        {
            public Validator()
            {
                RuleFor(r => r.SetId).GreaterThan(0);
                RuleFor(r => r.PrototypeId).GreaterThan(0);
            }
        }
    }
}