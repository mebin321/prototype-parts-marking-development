namespace WebApi.Features.PrototypeSets.Requests
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

    public class ListPrototypeSetsQuery : IRequest<PagedDataDto<PrototypeSetDto>>
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

        [SwaggerParameter("Filtering condition for active prototype sets.", Required = false)]
        public bool? IsActive { get; set; }

        [SwaggerParameter("List of outlet codes to which prototype sets belongs.", Required = false)]
        public List<string> OutletCodes { get; set; }

        [SwaggerParameter("List of outlet titles to which prototype sets belongs.", Required = false)]
        public List<string> OutletTitles { get; set; }

        [SwaggerParameter("List of product group codes to which prototype sets belongs.", Required = false)]
        public List<string> ProductGroupCodes { get; set; }

        [SwaggerParameter("List of product group titles to which prototype sets belongs.", Required = false)]
        public List<string> ProductGroupTitles { get; set; }

        [SwaggerParameter("List of gate level codes to which prototype sets belongs.", Required = false)]
        public List<string> GateLevelCodes { get; set; }

        [SwaggerParameter("List of gate level titles to which prototype sets belongs.", Required = false)]
        public List<string> GateLevelTitles { get; set; }

        [SwaggerParameter("List of location codes to which prototype sets belongs.", Required = false)]
        public List<string> LocationCodes { get; set; }

        [SwaggerParameter("List of location titles to which prototype sets belongs.", Required = false)]
        public List<string> LocationTitles { get; set; }

        [SwaggerParameter("List of evidence year codes to which prototype sets belongs.", Required = false)]
        public List<string> EvidenceYearCodes { get; set; }

        [SwaggerParameter("List of evidence year titles to which prototype sets belongs.", Required = false)]
        public List<int> EvidenceYearTitles { get; set; }

        [SwaggerParameter("Lower limit of evidence year to which prototype sets belong.", Required = false)]
        public int? EvidenceYearLowerLimit { get; set; }

        [SwaggerParameter("Upper limit of evidence year of creation to which prototype sets belong.", Required = false)]
        public int? EvidenceYearUpperLimit { get; set; }

        [SwaggerParameter("List of set identifiers to which prototype sets belongs.", Required = false)]
        public List<string> SetIdentifiers { get; set; }

        [SwaggerParameter("List of customers to which prototype sets belongs.", Required = false)]
        public List<string> Customers { get; set; }

        [SwaggerParameter("List of set projects to which prototype sets belongs.", Required = false)]
        public List<string> Projects { get; set; }

        [SwaggerParameter("List of set project numbers to which prototype sets belongs.", Required = false)]
        public List<string> ProjectNumbers { get; set; }

        [SwaggerParameter("List of user ids which created prototype sets.", Required = false)]
        public List<int> CreatedBy { get; set; }

        [SwaggerParameter("List of user ids which modified prototype sets.", Required = false)]
        public List<int> ModifiedBy { get; set; }

        [SwaggerParameter("List of user ids which deleted prototype sets.", Required = false)]
        public List<int> DeletedBy { get; set; }

        [SwaggerParameter("Lower date and time of creation to which prototype sets belong.", Required = false)]
        public DateTimeOffset? CreatedAtLowerLimit { get; set; }

        [SwaggerParameter("Upper time of creation to which prototype sets belong.", Required = false)]
        public DateTimeOffset? CreatedAtUpperLimit { get; set; }

        [SwaggerParameter("Lower time limit of modification to which prototype sets belong.", Required = false)]
        public DateTimeOffset? ModifiedAtLowerLimit { get; set; }

        [SwaggerParameter("Upper time limit of modification to which prototype sets belong.", Required = false)]
        public DateTimeOffset? ModifiedAtUpperLimit { get; set; }

        [SwaggerParameter("Lower time limit of deletion to which prototype sets belong.", Required = false)]
        public DateTimeOffset? DeletedAtLowerLimit { get; set; }

        [SwaggerParameter("Upper time limit of deletion to which prototype sets belong.", Required = false)]
        public DateTimeOffset? DeletedAtUpperLimit { get; set; }

        public string SortBy { get; set; }

        public string SortDirection { get; set; }

        public class Handler : IRequestHandler<ListPrototypeSetsQuery, PagedDataDto<PrototypeSetDto>>
        {
            private readonly IDbContextFactory<PrototypePartsDbContext> dbContextFactory;
            private readonly ISortColumnMapping<PrototypeSet> sortColumnMapping;

            public Handler(IDbContextFactory<PrototypePartsDbContext> dbContextFactory, ISortColumnMapping<PrototypeSet> sortColumnMapping)
            {
                Guard.NotNull(dbContextFactory, nameof(dbContextFactory));
                Guard.NotNull(sortColumnMapping, nameof(sortColumnMapping));

                this.dbContextFactory = dbContextFactory;
                this.sortColumnMapping = sortColumnMapping;
            }

            public async Task<PagedDataDto<PrototypeSetDto>> Handle(
                ListPrototypeSetsQuery request,
                CancellationToken cancellationToken)
            {
                await using var dbContext = dbContextFactory.CreateDbContext();

                var query = dbContext.PrototypeSets
                    .AsNoTracking()
                    .Include(s => s.CreatedBy)
                    .Include(s => s.ModifiedBy)
                    .Include(s => s.DeletedBy)
                    .SortWith(request.SortBy, request.SortDirection, s => s.Id, sortColumnMapping)
                    .FilterWith(request.IsActive)
                    .AsQueryable();

                query = Filter(query, request.OutletCodes, s => request.OutletCodes.Contains(s.OutletCode));
                query = Filter(query, request.OutletTitles, s => request.OutletTitles.Contains(s.OutletTitle));
                query = Filter(query, request.ProductGroupCodes, s => request.ProductGroupCodes.Contains(s.ProductGroupCode));
                query = Filter(query, request.ProductGroupTitles, s => request.ProductGroupTitles.Contains(s.ProductGroupTitle));
                query = Filter(query, request.GateLevelCodes, s => request.GateLevelCodes.Contains(s.GateLevelCode));
                query = Filter(query, request.GateLevelTitles, s => request.GateLevelTitles.Contains(s.GateLevelTitle));
                query = Filter(query, request.LocationCodes, s => request.LocationCodes.Contains(s.LocationCode));
                query = Filter(query, request.LocationTitles, s => request.LocationTitles.Contains(s.LocationTitle));
                query = Filter(query, request.EvidenceYearCodes, s => request.EvidenceYearCodes.Contains(s.EvidenceYearCode));
                query = Filter(query, request.EvidenceYearTitles, s => request.EvidenceYearTitles.Contains(s.EvidenceYearTitle));
                query = Filter(query, request.SetIdentifiers, s => request.SetIdentifiers.Contains(s.SetIdentifier));
                query = Filter(query, request.Customers, s => request.Customers.Contains(s.Customer));
                query = Filter(query, request.Projects, s => request.Projects.Contains(s.Project));
                query = Filter(query, request.ProjectNumbers, s => request.ProjectNumbers.Contains(s.ProjectNumber));

                query = Filter(query, request.CreatedBy, s => request.CreatedBy.Contains(s.CreatedById));
                query = Filter(query, request.ModifiedBy, s => request.ModifiedBy.Contains(s.ModifiedById));
                query = Filter(query, request.DeletedBy, s => request.DeletedBy.Contains(s.DeletedById.Value));

                query = Filter(query, request.EvidenceYearLowerLimit, s => request.EvidenceYearLowerLimit.Value <= s.EvidenceYearTitle);
                query = Filter(query, request.EvidenceYearUpperLimit, s => request.EvidenceYearUpperLimit.Value >= s.EvidenceYearTitle);

                query = Filter(query, request.CreatedAtLowerLimit, s => request.CreatedAtLowerLimit.Value <= s.CreatedAt);
                query = Filter(query, request.CreatedAtUpperLimit, s => request.CreatedAtUpperLimit.Value >= s.CreatedAt);

                query = Filter(query, request.ModifiedAtLowerLimit, s => request.ModifiedAtLowerLimit.Value <= s.ModifiedAt);
                query = Filter(query, request.ModifiedAtUpperLimit, s => request.ModifiedAtUpperLimit.Value >= s.ModifiedAt);

                query = Filter(query, request.DeletedAtLowerLimit, s => request.DeletedAtLowerLimit.Value <= s.DeletedAt);
                query = Filter(query, request.DeletedAtUpperLimit, s => request.DeletedAtUpperLimit.Value >= s.DeletedAt);

                var prototypes = await PagedList.CreateAsync(query, request.Page, request.PageSize);

                return new PagedDataDto<PrototypeSetDto>
                {
                    Items = prototypes.Select(p => PrototypeSetDto.From(p)).ToList(),
                    Pagination = prototypes.CreatePagination(),
                };
            }

            private IQueryable<PrototypeSet> Filter(IQueryable<PrototypeSet> query, object valueToCompare, Expression<Func<PrototypeSet, bool>> predicate)
            {
                if (valueToCompare != null)
                {
                    query = query.Where(predicate);
                }

                return query;
            }
        }

        public class Validator : AbstractValidator<ListPrototypeSetsQuery>
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
                RuleForEach(r => r.SetIdentifiers).NotEmpty();
                RuleForEach(r => r.Customers).NotEmpty();
                RuleForEach(r => r.Projects).NotEmpty();
                RuleForEach(r => r.ProjectNumbers).NotEmpty();
                RuleForEach(r => r.CreatedBy).NotEmpty();
                RuleForEach(r => r.ModifiedBy).NotEmpty();
                RuleForEach(r => r.DeletedBy).NotEmpty();
            }
        }
    }
}