namespace WebApi.Features.PrototypeVariants.Requests
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Authentication.Services;
    using Data;
    using FluentValidation;
    using MediatR;
    using Microsoft.EntityFrameworkCore;
    using Models;
    using Utilities;
    using WebApi.Common.ResourceVersioning;

    public class CreatePrototypeVariantCommand : IRequest<PrototypeVariantDto>
    {
        public int SetId { get; set; }

        public int PrototypeId { get; set; }

        public string Comment { get; set; }

        public class Handler : IRequestHandler<CreatePrototypeVariantCommand, PrototypeVariantDto>
        {
            private readonly IDbContextFactory<PrototypePartsDbContext> dbContextFactory;
            private readonly IProblemDetailsFactory problemDetailsFactory;
            private readonly ICurrentUserAccessor currentUserAccessor;
            private readonly IResourceVersionManager resourceVersionManager;

            public Handler(
                IDbContextFactory<PrototypePartsDbContext> dbContextFactory,
                IProblemDetailsFactory problemDetailsFactory,
                ICurrentUserAccessor currentUserAccessor,
                IResourceVersionManager resourceVersionManager)
            {
                Guard.NotNull(dbContextFactory, nameof(dbContextFactory));
                Guard.NotNull(problemDetailsFactory, nameof(problemDetailsFactory));

                this.dbContextFactory = dbContextFactory;
                this.problemDetailsFactory = problemDetailsFactory;
                this.currentUserAccessor = currentUserAccessor;
                this.resourceVersionManager = resourceVersionManager;
            }

            public async Task<PrototypeVariantDto> Handle(
                CreatePrototypeVariantCommand request,
                CancellationToken cancellationToken)
            {
                await using var dbContext = dbContextFactory.CreateDbContext();

                var prototype = await GetPrototypeAsync(dbContext, request.SetId, request.PrototypeId);

                resourceVersionManager.CheckVersion(prototype, true);

                var currentUserId = currentUserAccessor.GetCurrentUser();
                var currentUser = await dbContext.Users
                    .FirstOrDefaultAsync(u => u.Id == currentUserId, CancellationToken.None)
                    .ThrowIfNullAsync(problemDetailsFactory.EntityNotFound(nameof(User), currentUserId));

                var variant = new PrototypeVariant
                {
                    Prototype = prototype,
                    Version = prototype.PrototypeVariants.Count == 0 ? 1 : prototype.PrototypeVariants[0].Version + 1,
                    Comment = request.Comment,
                    CreatedBy = currentUser,
                    ModifiedBy = currentUser,
                };
                dbContext.PrototypeVariants.Add(variant);

                prototype.ModifiedBy = currentUser;
                dbContext.Entry(prototype).Property(p => p.ModifiedById).IsModified = true;

                await SaveDbContextChangesAsync(dbContext, prototype);

                return PrototypeVariantDto.From(variant);
            }

            private async Task<Prototype> GetPrototypeAsync(PrototypePartsDbContext dbContext, int setId, int prototypeId)
            {
                var set = await dbContext.PrototypeSets
                    .Include(s => s.Prototypes.Where(p => p.Id == prototypeId))
                    .ThenInclude(p => p.PrototypeVariants.OrderByDescending(v => v.Version))
                    .FirstOrDefaultAsync(s => s.Id == setId, CancellationToken.None)
                    .ThrowIfNullAsync(problemDetailsFactory.EntityNotFound(nameof(PrototypeSet), setId));

                if (set.Prototypes.Count == 0)
                {
                    throw problemDetailsFactory.EntityNotFound(nameof(Prototype), prototypeId);
                }

                return set.Prototypes[0];
            }

            private async Task SaveDbContextChangesAsync(PrototypePartsDbContext dbContext, Prototype prototype)
            {
                try
                {
                    await dbContext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw new ConflictException(
                        problemDetailsFactory.Conflict(
                            "Prototype old version.",
                            $"Could not update old version of Prototype whit Id {prototype.Id}."));
                }
            }
        }

        public class Validator : AbstractValidator<CreatePrototypeVariantCommand>
        {
            public Validator()
            {
                RuleFor(r => r.SetId).GreaterThan(0);
                RuleFor(r => r.PrototypeId).GreaterThan(0);
                RuleFor(r => r.Comment).NotEmpty();
            }
        }
    }
}