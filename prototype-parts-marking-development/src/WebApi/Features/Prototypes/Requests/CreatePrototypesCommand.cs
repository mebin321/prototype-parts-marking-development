namespace WebApi.Features.Prototypes.Requests
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Authentication.Services;
    using Common.ResourceVersioning;
    using Data;
    using FluentValidation;
    using MediatR;
    using Microsoft.EntityFrameworkCore;
    using Models;
    using Services;
    using Utilities;

    public class CreatePrototypesCommand : IRequest<List<CreatedPrototypeDto>>
    {
        [Required]
        public int SetId { get; set; }

        [Required]
        public List<CreatePrototypesRequestDto> Prototypes { get; set; }

        public class Handler : IRequestHandler<CreatePrototypesCommand, List<CreatedPrototypeDto>>
        {
            private readonly IDbContextFactory<PrototypePartsDbContext> dbContextFactory;
            private readonly IProblemDetailsFactory problemDetailsFactory;
            private readonly ICurrentUserAccessor currentUserAccessor;
            private readonly IResourceVersionManager resourceVersionManager;
            private readonly IUrlCreator urlCreator;

            public Handler(
                IDbContextFactory<PrototypePartsDbContext> dbContextFactory,
                IProblemDetailsFactory problemDetailsFactory,
                ICurrentUserAccessor currentUserAccessor,
                IResourceVersionManager resourceVersionManager,
                IUrlCreator urlCreator)
            {
                Guard.NotNull(dbContextFactory, nameof(dbContextFactory));
                Guard.NotNull(problemDetailsFactory, nameof(problemDetailsFactory));

                this.dbContextFactory = dbContextFactory;
                this.problemDetailsFactory = problemDetailsFactory;
                this.currentUserAccessor = currentUserAccessor;
                this.resourceVersionManager = resourceVersionManager;
                this.urlCreator = urlCreator;
            }

            public async Task<List<CreatedPrototypeDto>> Handle(
                CreatePrototypesCommand request,
                CancellationToken cancellationToken)
            {
                await using var dbContext = dbContextFactory.CreateDbContext();

                var prototypeSet = await GetPrototypeSetAsync(dbContext, request.SetId);
                resourceVersionManager.CheckVersion(prototypeSet);

                var currentUser = await GetUserAsync(dbContext, currentUserAccessor.GetCurrentUser());

                var createdPrototypes = new List<Prototype>();

                foreach (var requestedPrototype in request.Prototypes)
                {
                    PrototypeCheckIndexUniqueness(prototypeSet, requestedPrototype.Index);

                    var part = await GetPartAsync(dbContext, requestedPrototype.PartMoniker);
                    var owner = await GetUserAsync(dbContext, requestedPrototype.OwnerId);

                    var prototype = new Prototype
                    {
                        PrototypeSetId = prototypeSet.Id,
                        PartTypeCode = part.Code,
                        PartTypeTitle = part.Title,
                        Type = PrototypeType.Original,
                        MaterialNumber = requestedPrototype.MaterialNumber,
                        RevisionCode = requestedPrototype.RevisionCode,
                        Index = requestedPrototype.Index,
                        Comment = requestedPrototype.Comment,
                        Owner = owner,
                        CreatedBy = currentUser,
                        ModifiedBy = currentUser,
                    };

                    createdPrototypes.Add(prototype);
                    prototypeSet.Prototypes.Add(prototype);
                }

                prototypeSet.ModifiedById = currentUser.Id;
                dbContext.Entry(prototypeSet).Property(p => p.ModifiedById).IsModified = true;

                await SaveDbContextChanges(dbContext, prototypeSet);

                return createdPrototypes
                    .Select(p => CreatedPrototypeDto.From(
                        p, urlCreator.CreateUrl(nameof(PrototypesController.GetPrototype), new { setId = p.PrototypeSetId, prototypeId = p.Id })))
                    .ToList();
            }

            private async Task<PrototypeSet> GetPrototypeSetAsync(PrototypePartsDbContext dbContext, int id)
            {
                var prototypeSet = await dbContext.PrototypeSets
                    .Include(s => s.Prototypes)
                    .SingleOrDefaultAsync(s => s.Id == id);

                if (prototypeSet is null)
                {
                    throw new NotFoundException(
                        problemDetailsFactory.NotFound(
                            "PrototypeSet not found.",
                            $"Could not find PrototypeSet with Id {id}."));
                }

                return prototypeSet;
            }

            private async Task<Part> GetPartAsync(PrototypePartsDbContext dbContext, string partMoniker)
            {
                var part = await dbContext.Parts
                    .AsNoTracking()
                    .SingleOrDefaultAsync(x => x.Moniker == partMoniker);

                if (part is null)
                {
                    throw new BadRequestException(
                        problemDetailsFactory.BadRequest(
                            "Part not found.",
                            $"Could not find Part with ID {partMoniker}."));
                }

                return part;
            }

            private void PrototypeCheckIndexUniqueness(PrototypeSet prototypeSet, int index)
            {
                var existingPrototype = prototypeSet.Prototypes.Find(p => p.Index == index && p.Type == PrototypeType.Original);
                if (existingPrototype != null)
                {
                    throw new BadRequestException(
                        problemDetailsFactory.BadRequest(
                            "Invalid Prototype.",
                            $"Invalid Index {index}. Prototype of type Original must have unique index."));
                }
            }

            private async Task<User> GetUserAsync(PrototypePartsDbContext dbContext, int id)
            {
                var user = await dbContext.Users
                    .SingleOrDefaultAsync(u => u.Id == id);

                if (user is null)
                {
                    throw new BadRequestException(
                        problemDetailsFactory.BadRequest(
                            "User not found.",
                            $"Could not find User with ID {id}."));
                }

                return user;
            }

            private async Task SaveDbContextChanges(PrototypePartsDbContext dbContext, PrototypeSet prototypeSet)
            {
                try
                {
                    await dbContext.SaveChangesAsync(CancellationToken.None);
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw new ConflictException(
                        problemDetailsFactory.Conflict(
                            "PrototypeSet old version.",
                            $"Could not update old version of PrototypeSet whit Id {prototypeSet.Id}."));
                }
            }
        }

        public class Validator : AbstractValidator<CreatePrototypesCommand>
        {
            public Validator()
            {
                RuleFor(r => r.SetId).NotEmpty();
                RuleFor(r => r.Prototypes).NotEmpty();
                RuleForEach(r => r.Prototypes)
                    .ChildRules(r => { r.RuleFor(x => x.PartMoniker).NotEmpty(); })
                    .ChildRules(r => { r.RuleFor(x => x.OwnerId).NotEmpty(); })
                    .ChildRules(r =>
                    {
                        r.RuleFor(x => x.Index).NotEmpty().GreaterThan(0).LessThanOrEqualTo(999);
                    })
                    .ChildRules(r =>
                    {
                        r.RuleFor(x => x.MaterialNumber)
                            .Cascade(CascadeMode.Stop)
                            .NotEmpty()
                            .Must(Validations.NotContainWhitespace).WithMessage("Whitespace characters are not allowed.")
                            .Length(13).WithMessage("Length of the Material Number must be exactly 13 characters.");
                    })
                    .ChildRules(r =>
                    {
                        r.RuleFor(x => x.RevisionCode)
                            .Cascade(CascadeMode.Stop)
                            .NotEmpty()
                            .Must(Validations.NotContainWhitespace).WithMessage("Whitespace characters are not allowed.")
                            .Must(Validations.BeNumeric).WithMessage("Revision Code must be numeric.")
                            .Length(2).WithMessage("Length of the Revision Code must be exactly 2 characters.");
                    });
            }
        }
    }
}