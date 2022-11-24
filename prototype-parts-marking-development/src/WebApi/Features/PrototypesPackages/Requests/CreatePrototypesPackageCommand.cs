namespace WebApi.Features.PrototypesPackages.Requests
{
    using System.ComponentModel.DataAnnotations;
    using System.Threading;
    using System.Threading.Tasks;
    using Authentication.Services;
    using Data;
    using FluentValidation;
    using MediatR;
    using Microsoft.EntityFrameworkCore;
    using Models;
    using Utilities;
    using WebApi.Common.PrototypeIdentifier;

    public class CreatePrototypesPackageCommand : IRequest<PrototypesPackageDto>
    {
        [Required]
        public string OutletMoniker { get; set; }

        [Required]
        public string ProductGroupMoniker { get; set; }

        [Required]
        public string LocationMoniker { get; set; }

        [Required]
        public string GateLevelMoniker { get; set; }

        [Required]
        public string PartMoniker { get; set; }

        [Required]
        public int EvidenceYear { get; set; }

        [Required]
        public int InitialCount { get; set; }

        [Required]
        public string Comment { get; set; }

        [Required]
        public int OwnerId { get; set; }

        public string Customer { get; set; }

        public string Project { get; set; }

        public string ProjectNumber { get; set; }

        public class Handler : IRequestHandler<CreatePrototypesPackageCommand, PrototypesPackageDto>
        {
            private readonly IDbContextFactory<PrototypePartsDbContext> dbContextFactory;
            private readonly IProblemDetailsFactory problemDetailsFactory;
            private readonly ICurrentUserAccessor currentUserAccessor;
            private readonly IPrototypeIdentifierGenerator prototypeIdentifierGenerator;

            public Handler(
                IDbContextFactory<PrototypePartsDbContext> dbContextFactory,
                IProblemDetailsFactory problemDetailsFactory,
                ICurrentUserAccessor currentUserAccessor,
                IPrototypeIdentifierGenerator prototypeIdentifierGenerator)
            {
                Guard.NotNull(dbContextFactory, nameof(dbContextFactory));
                Guard.NotNull(problemDetailsFactory, nameof(problemDetailsFactory));

                this.dbContextFactory = dbContextFactory;
                this.problemDetailsFactory = problemDetailsFactory;
                this.currentUserAccessor = currentUserAccessor;
                this.prototypeIdentifierGenerator = prototypeIdentifierGenerator;
            }

            public async Task<PrototypesPackageDto> Handle(
                CreatePrototypesPackageCommand request,
                CancellationToken cancellationToken)
            {
                await using var dbContext = dbContextFactory.CreateDbContext();

                var outlet = await GetOutletAsync(dbContext, request.OutletMoniker);
                var productGroup = await GetProductGroupAsync(dbContext, request.ProductGroupMoniker);
                var location = await GetLocationAsync(dbContext, request.LocationMoniker);
                var gateLevel = await GetGateLevelAsync(dbContext, request.GateLevelMoniker);
                var evidenceYear = await GetEvidenceYearAsync(dbContext, request.EvidenceYear);
                var part = await GetPartAsync(dbContext, request.PartMoniker);
                var user = await GetUserAsync(dbContext, currentUserAccessor.GetCurrentUser());
                var owner = await GetUserAsync(dbContext, request.OwnerId);

                string uniqueIdentifier;
                try
                {
                    uniqueIdentifier = await prototypeIdentifierGenerator.GenerateIdentifierFor(location.Id, evidenceYear.Id);
                }
                catch
                {
                    throw new BadRequestException(
                        problemDetailsFactory.BadRequest(
                            "Cannot create unique identifier",
                            $"Cannot create unique identifier for location: {location.Moniker} and year : {evidenceYear.Year}."));
                }

                var prototypesPackage = new PrototypesPackage
                {
                    OutletCode = outlet.Code,
                    OutletTitle = outlet.Title,
                    ProductGroupCode = productGroup.Code,
                    ProductGroupTitle = productGroup.Title,
                    GateLevelCode = gateLevel.Code,
                    GateLevelTitle = gateLevel.Title,
                    LocationCode = location.Code,
                    LocationTitle = location.Title,
                    EvidenceYearCode = evidenceYear.Code,
                    EvidenceYearTitle = evidenceYear.Year,
                    PartTypeCode = part.Code,
                    PartTypeTitle = part.Title,
                    PackageIdentifier = uniqueIdentifier,
                    InitialCount = request.InitialCount,
                    ActualCount = request.InitialCount,
                    Comment = request.Comment,
                    Owner = owner,
                    CreatedBy = user,
                    ModifiedBy = user,
                    Customer = request.Customer,
                    Project = request.Project,
                    ProjectNumber = request.ProjectNumber,
                };

                dbContext.PrototypesPackages.Add(prototypesPackage);
                await dbContext.SaveChangesAsync();

                return PrototypesPackageDto.From(prototypesPackage);
            }

            private async Task<Outlet> GetOutletAsync(PrototypePartsDbContext dbContext, string outletMoniker)
            {
                var outlet = await dbContext.Outlets.AsNoTracking()
                    .SingleOrDefaultAsync(x => x.Moniker == outletMoniker);

                if (outlet is null)
                {
                    throw new NotFoundException(problemDetailsFactory.NotFound(
                            "Outlet not found.",
                            $"Could not find Outlet with ID {outletMoniker}."));
                }

                return outlet;
            }

            private async Task<ProductGroup> GetProductGroupAsync(
                PrototypePartsDbContext dbContext,
                string productGroupMoniker)
            {
                var productGroup = await dbContext.ProductGroups.AsNoTracking()
                    .SingleOrDefaultAsync(x => x.Moniker == productGroupMoniker);

                if (productGroup is null)
                {
                    throw new NotFoundException(problemDetailsFactory.NotFound(
                            "ProductGroup not found.",
                            $"Could not find ProductGroup with ID {productGroupMoniker}."));
                }

                return productGroup;
            }

            private async Task<Location> GetLocationAsync(PrototypePartsDbContext dbContext, string locationMoniker)
            {
                var location = await dbContext.Locations.AsNoTracking()
                    .SingleOrDefaultAsync(x => x.Moniker == locationMoniker);

                if (location is null)
                {
                    throw new NotFoundException(problemDetailsFactory.NotFound(
                            "Location not found.",
                            $"Could not find Location with ID {locationMoniker}."));
                }

                return location;
            }

            private async Task<GateLevel> GetGateLevelAsync(PrototypePartsDbContext dbContext, string gateLevelMoniker)
            {
                var gateLevel = await dbContext.GateLevels.AsNoTracking()
                    .SingleOrDefaultAsync(x => x.Moniker == gateLevelMoniker);

                if (gateLevel is null)
                {
                    throw new NotFoundException(problemDetailsFactory.NotFound(
                            "GateLevel not found.",
                            $"Could not find GateLevel with ID {gateLevelMoniker}."));
                }

                return gateLevel;
            }

            private async Task<Part> GetPartAsync(PrototypePartsDbContext dbContext, string partMoniker)
            {
                var part = await dbContext.Parts.AsNoTracking()
                    .SingleOrDefaultAsync(p => p.Moniker == partMoniker);

                if (part is null)
                {
                    throw new NotFoundException(problemDetailsFactory.NotFound(
                        "Part not found.",
                        $"Could not find Part with ID {partMoniker}."));
                }

                return part;
            }

            private async Task<EvidenceYear> GetEvidenceYearAsync(PrototypePartsDbContext dbContext, int year)
            {
                var yearOfEvidence = await dbContext.EvidenceYears.AsNoTracking()
                    .SingleOrDefaultAsync(y => y.Year == year);

                if (yearOfEvidence is null)
                {
                    throw new NotFoundException(problemDetailsFactory.NotFound(
                            "EvidenceYear not found.",
                            $"Could not find EvidenceYear with ID {year}."));
                }

                return yearOfEvidence;
            }

            private async Task<User> GetUserAsync(PrototypePartsDbContext dbContext, int id)
            {
                var user = await dbContext.Users
                    .SingleOrDefaultAsync(u => u.Id == id);

                if (user is null)
                {
                    throw new NotFoundException(problemDetailsFactory.NotFound(
                            "User not found.",
                            $"Could not find User with ID {id}."));
                }

                return user;
            }
        }

        public class Validator : AbstractValidator<CreatePrototypesPackageCommand>
        {
            public Validator()
            {
                RuleFor(r => r.OutletMoniker).NotEmpty()
                    .Must(Validations.NotContainWhitespace).WithMessage("Whitespace characters are not allowed.");

                RuleFor(r => r.ProductGroupMoniker).NotEmpty()
                    .Must(Validations.NotContainWhitespace).WithMessage("Whitespace characters are not allowed.");

                RuleFor(r => r.GateLevelMoniker).NotEmpty()
                    .Must(Validations.NotContainWhitespace).WithMessage("Whitespace characters are not allowed.");

                RuleFor(r => r.LocationMoniker).NotEmpty()
                    .Must(Validations.NotContainWhitespace).WithMessage("Whitespace characters are not allowed.");

                RuleFor(r => r.PartMoniker).NotEmpty()
                    .Must(Validations.NotContainWhitespace).WithMessage("Whitespace characters are not allowed.");

                RuleFor(r => r.EvidenceYear).NotEmpty();

                RuleFor(r => r.InitialCount).GreaterThan(0).LessThanOrEqualTo(999);

                RuleFor(r => r.OwnerId).GreaterThan(0);

                RuleFor(r => r.Customer).NotEmpty();
                RuleFor(r => r.Project).NotEmpty();
                RuleFor(r => r.ProjectNumber).NotEmpty();
            }
        }
    }
}