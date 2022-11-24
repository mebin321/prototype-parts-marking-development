namespace WebApi.Features.PrototypesPackages.Requests
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

    public class ListPrototypesPackagesQuery : IRequest<PagedDataDto<PrototypesPackageDto>>
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

        [SwaggerParameter("String used for fulltext search in comment.", Required = false)]
        public string Search { get; set; }

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

        [SwaggerParameter("List of part type codes to which prototype sets belongs.", Required = false)]
        public List<string> PartTypeCodes { get; set; }

        [SwaggerParameter("List of part type titles to which prototype sets belongs.", Required = false)]
        public List<string> PartTypeTitles { get; set; }

        [SwaggerParameter("List of package identifiers to which prototype sets belongs.", Required = false)]
        public List<string> PackageIdentifiers { get; set; }

        [SwaggerParameter("List of customers to which prototype sets belongs.", Required = false)]
        public List<string> Customers { get; set; }

        [SwaggerParameter("List of set projects to which prototype sets belongs.", Required = false)]
        public List<string> Projects { get; set; }

        [SwaggerParameter("List of set project numbers to which prototype sets belongs.", Required = false)]
        public List<string> ProjectNumbers { get; set; }

        [SwaggerParameter("List of initial counts to which prototype sets belongs.", Required = false)]
        public List<int> InitialCounts { get; set; }

        [SwaggerParameter("Lower limit of initial count to which prototype sets belong.", Required = false)]
        public int? InitialCountLowerLimit { get; set; }

        [SwaggerParameter("Upper limit of initial count to which prototype sets belong.", Required = false)]
        public int? InitialCountUpperLimit { get; set; }

        [SwaggerParameter("List of actual counts to which prototype sets belongs.", Required = false)]
        public List<int> ActualCounts { get; set; }

        [SwaggerParameter("Lower limit of actual counts to which prototype sets belong.", Required = false)]
        public int? ActualCountLowerLimit { get; set; }

        [SwaggerParameter("Upper limit of actual count to which prototype sets belong.", Required = false)]
        public int? ActualCountUpperLimit { get; set; }

        [SwaggerParameter("List of user ids which own prototype sets.", Required = false)]
        public List<int> Owners { get; set; }

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

        public class Handler : IRequestHandler<ListPrototypesPackagesQuery, PagedDataDto<PrototypesPackageDto>>
        {
            private readonly IDbContextFactory<PrototypePartsDbContext> dbContextFactory;
            private readonly ISortColumnMapping<PrototypesPackage> sortColumnMapping;

            public Handler(IDbContextFactory<PrototypePartsDbContext> dbContextFactory, ISortColumnMapping<PrototypesPackage> sortColumnMapping)
            {
                Guard.NotNull(dbContextFactory, nameof(dbContextFactory));
                Guard.NotNull(sortColumnMapping, nameof(sortColumnMapping));

                this.dbContextFactory = dbContextFactory;
                this.sortColumnMapping = sortColumnMapping;
            }

            public async Task<PagedDataDto<PrototypesPackageDto>> Handle(ListPrototypesPackagesQuery request, CancellationToken cancellationToken)
            {
                await using var dbContext = dbContextFactory.CreateDbContext();

                var query = dbContext.PrototypesPackages
                    .AsNoTracking()
                    .Include(p => p.Owner)
                    .Include(p => p.CreatedBy)
                    .Include(p => p.ModifiedBy)
                    .Include(p => p.DeletedBy)
                    .SortWith(request.SortBy, request.SortDirection, p => p.Id, sortColumnMapping)
                    .FilterWith(request.IsActive)
                    .AsQueryable();

                query = Filter(query, request.Search, p => EF.Functions.ILike(p.Comment, $"%{request.Search}%"));

                query = Filter(query, request.OutletCodes, p => request.OutletCodes.Contains(p.OutletCode));
                query = Filter(query, request.OutletTitles, p => request.OutletTitles.Contains(p.OutletTitle));
                query = Filter(query, request.ProductGroupCodes, p => request.ProductGroupCodes.Contains(p.ProductGroupCode));
                query = Filter(query, request.ProductGroupTitles, p => request.ProductGroupTitles.Contains(p.ProductGroupTitle));
                query = Filter(query, request.GateLevelCodes, p => request.GateLevelCodes.Contains(p.GateLevelCode));
                query = Filter(query, request.GateLevelTitles, p => request.GateLevelTitles.Contains(p.GateLevelTitle));
                query = Filter(query, request.LocationCodes, p => request.LocationCodes.Contains(p.LocationCode));
                query = Filter(query, request.LocationTitles, p => request.LocationTitles.Contains(p.LocationTitle));
                query = Filter(query, request.EvidenceYearCodes, p => request.EvidenceYearCodes.Contains(p.EvidenceYearCode));
                query = Filter(query, request.EvidenceYearTitles, p => request.EvidenceYearTitles.Contains(p.EvidenceYearTitle));
                query = Filter(query, request.PartTypeCodes, p => request.PartTypeCodes.Contains(p.PartTypeCode));
                query = Filter(query, request.PartTypeTitles, p => request.PartTypeTitles.Contains(p.PartTypeTitle));
                query = Filter(query, request.PackageIdentifiers, p => request.PackageIdentifiers.Contains(p.PackageIdentifier));
                query = Filter(query, request.InitialCounts, p => request.InitialCounts.Contains(p.InitialCount));
                query = Filter(query, request.ActualCounts, p => request.ActualCounts.Contains(p.ActualCount));
                query = Filter(query, request.Owners, p => request.Owners.Contains(p.OwnerId));
                query = Filter(query, request.Customers, p => request.Customers.Contains(p.Customer));
                query = Filter(query, request.Projects, p => request.Projects.Contains(p.Project));
                query = Filter(query, request.ProjectNumbers, p => request.ProjectNumbers.Contains(p.ProjectNumber));

                query = Filter(query, request.CreatedBy, p => request.CreatedBy.Contains(p.CreatedById));
                query = Filter(query, request.ModifiedBy, p => request.ModifiedBy.Contains(p.ModifiedById));
                query = Filter(query, request.DeletedBy, p => request.DeletedBy.Contains(p.DeletedById.Value));

                query = Filter(query, request.EvidenceYearLowerLimit, p => request.EvidenceYearLowerLimit.Value <= p.EvidenceYearTitle);
                query = Filter(query, request.EvidenceYearUpperLimit, p => request.EvidenceYearUpperLimit.Value >= p.EvidenceYearTitle);

                query = Filter(query, request.InitialCountLowerLimit, p => request.InitialCountLowerLimit.Value <= p.InitialCount);
                query = Filter(query, request.InitialCountUpperLimit, p => request.InitialCountUpperLimit.Value >= p.InitialCount);

                query = Filter(query, request.ActualCountLowerLimit, p => request.ActualCountLowerLimit.Value <= p.ActualCount);
                query = Filter(query, request.ActualCountUpperLimit, p => request.ActualCountUpperLimit.Value >= p.ActualCount);

                query = Filter(query, request.CreatedAtLowerLimit, p => request.CreatedAtLowerLimit.Value <= p.CreatedAt);
                query = Filter(query, request.CreatedAtUpperLimit, p => request.CreatedAtUpperLimit.Value >= p.CreatedAt);

                query = Filter(query, request.ModifiedAtLowerLimit, p => request.ModifiedAtLowerLimit.Value <= p.ModifiedAt);
                query = Filter(query, request.ModifiedAtUpperLimit, p => request.ModifiedAtUpperLimit.Value >= p.ModifiedAt);

                query = Filter(query, request.DeletedAtLowerLimit, p => request.DeletedAtLowerLimit.Value <= p.DeletedAt);
                query = Filter(query, request.DeletedAtUpperLimit, p => request.DeletedAtUpperLimit.Value >= p.DeletedAt);

                var prototypesPackages = await PagedList.CreateAsync(query, request.Page, request.PageSize);

                return new PagedDataDto<PrototypesPackageDto>
                {
                    Items = prototypesPackages.Select(p => PrototypesPackageDto.From(p)).ToList(),
                    Pagination = prototypesPackages.CreatePagination(),
                };
            }

            private IQueryable<PrototypesPackage> Filter(IQueryable<PrototypesPackage> query, object valueToCompare, Expression<Func<PrototypesPackage, bool>> predicate)
            {
                if (valueToCompare != null)
                {
                    query = query.Where(predicate);
                }

                return query;
            }
        }

        public class Validator : AbstractValidator<ListPrototypesPackagesQuery>
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
                RuleForEach(r => r.PackageIdentifiers).NotEmpty();
                RuleForEach(r => r.Customers).NotEmpty();
                RuleForEach(r => r.Projects).NotEmpty();
                RuleForEach(r => r.ProjectNumbers).NotEmpty();
                RuleForEach(r => r.InitialCounts).NotEmpty();
                RuleForEach(r => r.ActualCounts).NotEmpty();
                RuleForEach(r => r.Owners).NotEmpty();
                RuleForEach(r => r.CreatedBy).NotEmpty();
                RuleForEach(r => r.ModifiedBy).NotEmpty();
                RuleForEach(r => r.DeletedBy).NotEmpty();
            }
        }
    }
}