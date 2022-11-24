namespace WebApi.Features.Prototypes.Requests
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;
    using Common.Paging;
    using Data;
    using FluentValidation;
    using MediatR;
    using Microsoft.EntityFrameworkCore;
    using Models;
    using Swashbuckle.AspNetCore.Annotations;
    using Utilities;
    using WebApi.Common;
    using WebApi.Common.Sorting;

    public class FilteredListPrototypesQuery : IRequest<PagedDataDto<EnrichedPrototypeDto>>
    {
        private const int MaxPageSize = 20;

        private int page = 1;
        private int pageSize = 10;

        [Range(1, double.PositiveInfinity)]
        [SwaggerParameter("Page to retrieve.", Required = false)]
        public int Page
        {
            get => page;
            set => page = Math.Max(1, value);
        }

        [Range(1, MaxPageSize)]
        [SwaggerParameter("Page size.", Required = false)]
        public int PageSize
        {
            get => pageSize;
            set => pageSize = Math.Clamp(value, 1, MaxPageSize);
        }

        [SwaggerParameter("Filtering condition for active prototypes.", Required = false)]
        public bool? IsActive { get; set; }

        [SwaggerParameter("String used for fulltext search in comment.", Required = false)]
        public string Search { get; set; }

        [SwaggerParameter("List of outlet codes to which prototypes belongs.", Required = false)]
        public List<string> OutletCodes { get; set; }

        [SwaggerParameter("List of outlet titles to which prototypes belongs.", Required = false)]
        public List<string> OutletTitles { get; set; }

        [SwaggerParameter("List of product group codes to which prototypes belongs.", Required = false)]
        public List<string> ProductGroupCodes { get; set; }

        [SwaggerParameter("List of product group titles to which prototypes belongs.", Required = false)]
        public List<string> ProductGroupTitles { get; set; }

        [SwaggerParameter("List of gate level codes to which prototypes belongs.", Required = false)]
        public List<string> GateLevelCodes { get; set; }

        [SwaggerParameter("List of gate level titles to which prototypes belongs.", Required = false)]
        public List<string> GateLevelTitles { get; set; }

        [SwaggerParameter("List of location codes to which prototypes belongs.", Required = false)]
        public List<string> LocationCodes { get; set; }

        [SwaggerParameter("List of location titles to which prototypes belongs.", Required = false)]
        public List<string> LocationTitles { get; set; }

        [SwaggerParameter("List of evidence year codes to which prototypes belongs.", Required = false)]
        public List<string> EvidenceYearCodes { get; set; }

        [SwaggerParameter("List of evidence year titles to which prototypes belongs.", Required = false)]
        public List<int> EvidenceYearTitles { get; set; }

        [SwaggerParameter("Lower limit of evidence year to which prototypes belong.", Required = false)]
        public int? EvidenceYearLowerLimit { get; set; }

        [SwaggerParameter("Upper limit of evidence year of creation to which prototypes belong.", Required = false)]
        public int? EvidenceYearUpperLimit { get; set; }

        [SwaggerParameter("List of part type codes to which prototypes belongs.", Required = false)]
        public List<string> PartTypeCodes { get; set; }

        [SwaggerParameter("List of part type titles to which prototypes belongs.", Required = false)]
        public List<string> PartTypeTitles { get; set; }

        [SwaggerParameter("List of types to which prototypes belongs.", Required = false)]
        public string Type { get; set; }

        [SwaggerParameter("List of set identifiers which prototypes belongs.", Required = false)]
        public List<string> SetIdentifiers { get; set; }

        [SwaggerParameter("List of customers to which prototypes belongs.", Required = false)]
        public List<string> Customers { get; set; }

        [SwaggerParameter("List of set projects to which prototypes belongs.", Required = false)]
        public List<string> Projects { get; set; }

        [SwaggerParameter("List of index to which prototypes belongs.", Required = false)]
        public List<int> Indexes { get; set; }

        [SwaggerParameter("Lower limit of index to which prototypes belong.", Required = false)]
        public int? IndexLowerLimit { get; set; }

        [SwaggerParameter("Upper limit of index to which prototypes belong.", Required = false)]
        public int? IndexUpperLimit { get; set; }

        [SwaggerParameter("List of set project numbers to which prototypes belongs.", Required = false)]
        public List<string> ProjectNumbers { get; set; }

        [SwaggerParameter("List of set material codes which prototypes belongs.", Required = false)]
        public List<string> MaterialNumbers { get; set; }

        [SwaggerParameter("List of set revision codes which prototypes belongs.", Required = false)]
        public List<string> RevisionCodes { get; set; }

        [SwaggerParameter("List of prototype set ids which prototypes belongs.", Required = false)]
        public List<int> PrototypeSets { get; set; }

        [SwaggerParameter("List of user ids which own prototypes.", Required = false)]
        public List<int> Owners { get; set; }

        [SwaggerParameter("List of user ids which created prototypes.", Required = false)]
        public List<int> CreatedBy { get; set; }

        [SwaggerParameter("List of user ids which modified prototypes.", Required = false)]
        public List<int> ModifiedBy { get; set; }

        [SwaggerParameter("List of user ids which deleted prototypes.", Required = false)]
        public List<int> DeletedBy { get; set; }

        [SwaggerParameter("Lower date and time of creation to which prototypes belong.", Required = false)]
        public DateTimeOffset? CreatedAtLowerLimit { get; set; }

        [SwaggerParameter("Upper time of creation to which prototypes belong.", Required = false)]
        public DateTimeOffset? CreatedAtUpperLimit { get; set; }

        [SwaggerParameter("Lower time limit of modification to which prototypes belong.", Required = false)]
        public DateTimeOffset? ModifiedAtLowerLimit { get; set; }

        [SwaggerParameter("Upper time limit of modification to which prototypes belong.", Required = false)]
        public DateTimeOffset? ModifiedAtUpperLimit { get; set; }

        [SwaggerParameter("Lower time limit of deletion to which prototypes belong.", Required = false)]
        public DateTimeOffset? DeletedAtLowerLimit { get; set; }

        [SwaggerParameter("Upper time limit of deletion to which prototypes belong.", Required = false)]
        public DateTimeOffset? DeletedAtUpperLimit { get; set; }

        public string SortBy { get; set; }

        public string SortDirection { get; set; }

        public class Handler : IRequestHandler<FilteredListPrototypesQuery, PagedDataDto<EnrichedPrototypeDto>>
        {
            private readonly IDbContextFactory<PrototypePartsDbContext> dbContextFactory;
            private readonly ISortColumnMapping<Prototype> sortColumnMapping;

            public Handler(IDbContextFactory<PrototypePartsDbContext> dbContextFactory, ISortColumnMapping<Prototype> sortColumnMapping)
            {
                Guard.NotNull(dbContextFactory, nameof(dbContextFactory));
                Guard.NotNull(sortColumnMapping, nameof(sortColumnMapping));

                this.dbContextFactory = dbContextFactory;
                this.sortColumnMapping = sortColumnMapping;
            }

            public async Task<PagedDataDto<EnrichedPrototypeDto>> Handle(FilteredListPrototypesQuery request, CancellationToken cancellationToken)
            {
                await using var dbContext = dbContextFactory.CreateDbContext();

                var query = dbContext.Prototypes
                    .AsNoTracking()
                    .Include(p => p.PrototypeSet).ThenInclude(s => s.CreatedBy)
                    .Include(p => p.PrototypeSet).ThenInclude(s => s.ModifiedBy)
                    .Include(p => p.PrototypeSet).ThenInclude(s => s.DeletedBy)
                    .Include(p => p.Owner)
                    .Include(p => p.CreatedBy)
                    .Include(p => p.ModifiedBy)
                    .Include(p => p.DeletedBy)
                    .SortWith(request.SortBy, request.SortDirection, p => p.Id, sortColumnMapping)
                    .FilterWith(request.IsActive)
                    .AsQueryable();

                if (request.Type is not null)
                {
                   query = query.Where(p => p.Type == (PrototypeType)Enum.Parse(typeof(PrototypeType), request.Type));
                }

                query = Filter(query, request.Search, p => EF.Functions.ILike(p.Comment, $"%{request.Search}%"));

                query = Filter(query, request.OutletCodes, p => request.OutletCodes.Contains(p.PrototypeSet.OutletCode));
                query = Filter(query, request.OutletTitles, p => request.OutletTitles.Contains(p.PrototypeSet.OutletTitle));
                query = Filter(query, request.ProductGroupCodes, p => request.ProductGroupCodes.Contains(p.PrototypeSet.ProductGroupCode));
                query = Filter(query, request.ProductGroupTitles, p => request.ProductGroupTitles.Contains(p.PrototypeSet.ProductGroupTitle));
                query = Filter(query, request.GateLevelCodes, p => request.GateLevelCodes.Contains(p.PrototypeSet.GateLevelCode));
                query = Filter(query, request.GateLevelTitles, p => request.GateLevelTitles.Contains(p.PrototypeSet.GateLevelTitle));
                query = Filter(query, request.LocationCodes, p => request.LocationCodes.Contains(p.PrototypeSet.LocationCode));
                query = Filter(query, request.LocationTitles, p => request.LocationTitles.Contains(p.PrototypeSet.LocationTitle));
                query = Filter(query, request.EvidenceYearCodes, p => request.EvidenceYearCodes.Contains(p.PrototypeSet.EvidenceYearCode));
                query = Filter(query, request.EvidenceYearTitles, p => request.EvidenceYearTitles.Contains(p.PrototypeSet.EvidenceYearTitle));
                query = Filter(query, request.SetIdentifiers, p => request.SetIdentifiers.Contains(p.PrototypeSet.SetIdentifier));
                query = Filter(query, request.Customers, p => request.Customers.Contains(p.PrototypeSet.Customer));
                query = Filter(query, request.Projects, p => request.Projects.Contains(p.PrototypeSet.Project));
                query = Filter(query, request.ProjectNumbers, p => request.ProjectNumbers.Contains(p.PrototypeSet.ProjectNumber));

                query = Filter(query, request.PartTypeCodes, p => request.PartTypeCodes.Contains(p.PartTypeCode));
                query = Filter(query, request.PartTypeTitles, p => request.PartTypeTitles.Contains(p.PartTypeTitle));
                query = Filter(query, request.Indexes, p => request.Indexes.Contains(p.Index));
                query = Filter(query, request.MaterialNumbers, p => request.MaterialNumbers.Contains(p.MaterialNumber));
                query = Filter(query, request.RevisionCodes, p => request.RevisionCodes.Contains(p.RevisionCode));

                query = Filter(query, request.PrototypeSets, p => request.PrototypeSets.Contains(p.PrototypeSetId));
                query = Filter(query, request.Owners, p => request.Owners.Contains(p.OwnerId));
                query = Filter(query, request.CreatedBy, p => request.CreatedBy.Contains(p.CreatedById));
                query = Filter(query, request.ModifiedBy, p => request.ModifiedBy.Contains(p.ModifiedById));
                query = Filter(query, request.DeletedBy, p => request.DeletedBy.Contains(p.DeletedById.Value));

                query = Filter(query, request.EvidenceYearLowerLimit, p => request.EvidenceYearLowerLimit.Value <= p.PrototypeSet.EvidenceYearTitle);
                query = Filter(query, request.EvidenceYearUpperLimit, p => request.EvidenceYearUpperLimit.Value >= p.PrototypeSet.EvidenceYearTitle);

                query = Filter(query, request.IndexLowerLimit, p => request.IndexLowerLimit.Value <= p.Index);
                query = Filter(query, request.IndexUpperLimit, p => request.IndexUpperLimit.Value >= p.Index);

                query = Filter(query, request.CreatedAtLowerLimit, p => request.CreatedAtLowerLimit.Value <= p.CreatedAt);
                query = Filter(query, request.CreatedAtUpperLimit, p => request.CreatedAtUpperLimit.Value >= p.CreatedAt);

                query = Filter(query, request.ModifiedAtLowerLimit, p => request.ModifiedAtLowerLimit.Value <= p.ModifiedAt);
                query = Filter(query, request.ModifiedAtUpperLimit, p => request.ModifiedAtUpperLimit.Value >= p.ModifiedAt);

                query = Filter(query, request.DeletedAtLowerLimit, p => request.DeletedAtLowerLimit.Value <= p.DeletedAt);
                query = Filter(query, request.DeletedAtUpperLimit, p => request.DeletedAtUpperLimit.Value >= p.DeletedAt);

                var prototypes = await PagedList.CreateAsync(query, request.Page, request.PageSize);

                return new PagedDataDto<EnrichedPrototypeDto>
                {
                    Items = prototypes.Select(p => EnrichedPrototypeDto.From(p)).ToList(),
                    Pagination = prototypes.CreatePagination(),
                };
            }

            private IQueryable<Prototype> Filter(IQueryable<Prototype> query, object valueToCompare, Expression<Func<Prototype, bool>> predicate)
            {
                if (valueToCompare != null)
                {
                    query = query.Where(predicate);
                }

                return query;
            }
        }

        public class Validator : AbstractValidator<FilteredListPrototypesQuery>
        {
            public Validator()
            {
                RuleForEach(r => r.OutletCodes).NotEmpty();
                RuleForEach(r => r.OutletTitles).NotEmpty();
                RuleForEach(r => r.ProductGroupCodes).NotEmpty();
                RuleForEach(r => r.ProductGroupTitles).NotEmpty();
                RuleForEach(r => r.GateLevelCodes).NotEmpty();
                RuleForEach(r => r.GateLevelTitles).NotEmpty();
                RuleForEach(r => r.LocationCodes).NotEmpty();
                RuleForEach(r => r.LocationTitles).NotEmpty();
                RuleForEach(r => r.EvidenceYearCodes).NotEmpty();
                RuleForEach(r => r.EvidenceYearTitles).NotEmpty();
                RuleForEach(r => r.PartTypeCodes).NotEmpty();
                RuleForEach(r => r.PartTypeTitles).NotEmpty();
                RuleForEach(r => r.SetIdentifiers).NotEmpty();
                RuleForEach(r => r.Customers).NotEmpty();
                RuleForEach(r => r.Projects).NotEmpty();
                RuleForEach(r => r.ProjectNumbers).NotEmpty();
                RuleForEach(r => r.Indexes).NotEmpty();
                RuleForEach(r => r.MaterialNumbers).NotEmpty();
                RuleForEach(r => r.RevisionCodes).NotEmpty();
                RuleForEach(r => r.PrototypeSets).NotEmpty();
                RuleForEach(r => r.Owners).NotEmpty();
                RuleForEach(r => r.CreatedBy).NotEmpty();
                RuleForEach(r => r.ModifiedBy).NotEmpty();
                RuleForEach(r => r.DeletedBy).NotEmpty();

                string[] typeConditions =
                {
                    PrototypeType.Original.ToString(),
                    PrototypeType.Component.ToString(),
                    null,
                };

                RuleFor(r => r.Type).Must(t => typeConditions.Contains(t))
                    .WithMessage($"Type of prototype must be {typeConditions[0]}, {typeConditions[1]} or empty.");
            }
        }
    }
}