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

    public class CreateComponentsCommand : IRequest<List<CreatedPrototypeDto>>
    {
        [Required]
        public int SetId { get; set; }

        [Required]
        public List<CreatePrototypesRequestDto> Prototypes { get; set; }

        public class Handler : IRequestHandler<CreateComponentsCommand, List<CreatedPrototypeDto>>
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
                CreateComponentsCommand request,
                CancellationToken cancellationToken)
            {
                await using var dbContext = dbContextFactory.CreateDbContext();

                var prototypeSet = await GetPrototypeSetAsync(dbContext, request.SetId);
                resourceVersionManager.CheckVersion(prototypeSet);

                var currentUser = await GetUserAsync(dbContext, currentUserAccessor.GetCurrentUser());

                var createdPrototypes = new List<Prototype>();

                foreach (var requestedPrototype in request.Prototypes)
                {
                    var part = await GetPartAsync(dbContext, requestedPrototype.PartMoniker);

                    var owner = await GetUserAsync(dbContext, requestedPrototype.OwnerId);

                    var component = new Prototype
                    {
                        PrototypeSetId = prototypeSet.Id,
                        PartTypeCode = part.Code,
                        PartTypeTitle = part.Title,
                        Type = PrototypeType.Component,
                        MaterialNumber = requestedPrototype.MaterialNumber,
                        RevisionCode = requestedPrototype.RevisionCode,
                        Index = requestedPrototype.Index,
                        Comment = requestedPrototype.Comment,
                        Owner = owner,
                        CreatedBy = currentUser,
                        ModifiedBy = currentUser,
                    };

                    ValidateComponent(prototypeSet, component);

                    createdPrototypes.Add(component);
                    prototypeSet.Prototypes.Add(component);
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
                return await dbContext.PrototypeSets
                    .Include(s => s.Prototypes)
                    .SingleOrDefaultAsync(s => s.Id == id)
                    .ThrowIfNullAsync(problemDetailsFactory.EntityNotFound(nameof(PrototypeSet), id));
            }

            private async Task<Part> GetPartAsync(PrototypePartsDbContext dbContext, string partMoniker)
            {
                return await dbContext.Parts
                    .AsNoTracking()
                    .SingleOrDefaultAsync(x => x.Moniker == partMoniker, CancellationToken.None)
                    .ThrowIfNullAsync(problemDetailsFactory.EntityNotFound(nameof(Part), partMoniker));
            }

            private void ValidateComponent(PrototypeSet prototypeSet, Prototype component)
            {
                ValidatePartType(component);
                ValidateIndex(prototypeSet, component.Index);
                CheckUniqueness(prototypeSet, component);
            }

            private void ValidatePartType(Prototype component)
            {
                if (string.Equals(component.PartTypeCode, PartTypeCode.CompletePrototype))
                {
                    throw new BadRequestException(
                            problemDetailsFactory.BadRequest(
                                "Invalid Prototype.",
                                $"Invalid Part Type {component.PartTypeTitle}. Prototype of type Component cannot be a Complete Prototype."));
                }
            }

            private void ValidateIndex(PrototypeSet prototypeSet, int index)
            {
                var existingPrototype = prototypeSet.Prototypes.FirstOrDefault(
                    p => p.Index == index && p.Type == PrototypeType.Original);

                if (existingPrototype is null)
                {
                    throw new BadRequestException(
                        problemDetailsFactory.BadRequest(
                            "Invalid Prototype.",
                            $"Invalid Prototype Index {index}. Original Prototype with Index {index} does not exists."));
                }
            }

            private void CheckUniqueness(PrototypeSet prototypeSet, Prototype component)
            {
                var existingComponent = prototypeSet.Prototypes.FirstOrDefault(
                    p => p.Index == component.Index && p.PartTypeTitle == component.PartTypeTitle);

                if (existingComponent is not null)
                {
                    var details = $"Invalid Part Type {component.PartTypeTitle}." +
                                  $" Component with Index {component.Index} and Part Type {component.PartTypeTitle} already exists.";

                    throw new BadRequestException(problemDetailsFactory.BadRequest("Invalid Component.", details));
                }

                existingComponent = prototypeSet.Prototypes.FirstOrDefault(
                    p => p.Index == component.Index && p.PartTypeCode == component.PartTypeCode && p.DeletedAt == null);

                if (existingComponent is not null)
                {
                    var details = $"Invalid Part Type {component.PartTypeTitle}." +
                                  $" Active Component with Index {component.Index} and Part Code {component.PartTypeCode} already exists.";

                    throw new BadRequestException(problemDetailsFactory.BadRequest("Invalid Component.", details));
                }
            }

            private async Task<User> GetUserAsync(PrototypePartsDbContext dbContext, int id)
            {
                return await dbContext.Users
                    .SingleOrDefaultAsync(u => u.Id == id, CancellationToken.None)
                    .ThrowIfNullAsync(problemDetailsFactory.EntityNotFound(nameof(User), id));
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

        public class Validator : AbstractValidator<CreateComponentsCommand>
        {
            public Validator()
            {
                RuleFor(r => r.SetId).GreaterThan(0);
                RuleFor(r => r.Prototypes).NotEmpty();
                RuleForEach(r => r.Prototypes)
                    .ChildRules(r => { r.RuleFor(x => x.PartMoniker).NotEmpty(); })
                    .ChildRules(r => { r.RuleFor(x => x.OwnerId).GreaterThan(0); })
                    .ChildRules(r =>
                    {
                        r.RuleFor(x => x.Index).GreaterThan(0).LessThanOrEqualTo(999);
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