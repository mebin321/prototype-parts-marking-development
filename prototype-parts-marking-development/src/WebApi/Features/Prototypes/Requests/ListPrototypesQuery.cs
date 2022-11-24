namespace WebApi.Features.Prototypes.Requests
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Common.Paging;
    using Data;
    using FluentValidation;
    using MediatR;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Models;
    using Swashbuckle.AspNetCore.Annotations;
    using Utilities;
    using WebApi.Common;
    using WebApi.Common.Sorting;

    public class ListPrototypesQuery : IRequest<PagedDataDto<PrototypeDto>>
    {
        private const int MaxPageSize = 20;

        private int page = 1;
        private int pageSize = 10;

        [FromRoute(Name = "setId")]
        public int SetId { get; set; }

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

        public int? Index { get; set; }

        public string Type { get; set; }

        public bool? IsActive { get; set; }

        public string SortBy { get; set; }

        public string SortDirection { get; set; }

        public class Handler : IRequestHandler<ListPrototypesQuery, PagedDataDto<PrototypeDto>>
        {
            private readonly IDbContextFactory<PrototypePartsDbContext> dbContextFactory;

            private readonly IProblemDetailsFactory problemDetailsFactory;

            private readonly ISortColumnMapping<Prototype> sortColumnMapping;

            public Handler(IDbContextFactory<PrototypePartsDbContext> dbContextFactory, IProblemDetailsFactory problemDetailsFactory, ISortColumnMapping<Prototype> sortColumnMapping)
            {
                Guard.NotNull(dbContextFactory, nameof(dbContextFactory));
                Guard.NotNull(problemDetailsFactory, nameof(problemDetailsFactory));
                Guard.NotNull(sortColumnMapping, nameof(sortColumnMapping));

                this.dbContextFactory = dbContextFactory;
                this.problemDetailsFactory = problemDetailsFactory;
                this.sortColumnMapping = sortColumnMapping;
            }

            public async Task<PagedDataDto<PrototypeDto>> Handle(ListPrototypesQuery request, CancellationToken cancellationToken)
            {
                await using var dbContext = dbContextFactory.CreateDbContext();

                if (!dbContext.PrototypeSets.Any(s => s.Id == request.SetId))
                {
                    throw problemDetailsFactory.EntityNotFound(nameof(PrototypeSet), request.SetId);
                }

                var query = dbContext.Prototypes
                    .AsNoTracking()
                    .Include(s => s.Owner)
                    .Include(s => s.CreatedBy)
                    .Include(s => s.ModifiedBy)
                    .Include(s => s.DeletedBy)
                    .Where(p => p.PrototypeSetId == request.SetId)
                    .SortWith(request.SortBy, request.SortDirection, p => p.Id, sortColumnMapping)
                    .FilterWith(request.IsActive)
                    .AsQueryable();

                if (request.Index is not null)
                {
                    query = query.Where(p => p.Index == request.Index);
                }

                if (request.Type is not null)
                {
                    var type = (PrototypeType)Enum.Parse(typeof(PrototypeType), request.Type);
                    query = query.Where(p => p.Type == type);
                }

                var prototypes = await PagedList.CreateAsync(query, request.Page, request.PageSize);

                return new PagedDataDto<PrototypeDto>
                {
                    Items = prototypes.Select(p => PrototypeDto.From(p)).ToList(),
                    Pagination = prototypes.CreatePagination(),
                };
            }
        }

        public class Validator : AbstractValidator<ListPrototypesQuery>
        {
            public Validator()
            {
                string[] typeConditions =
                {
                    PrototypeType.Original.ToString(),
                    PrototypeType.Component.ToString(),
                    null,
                };

                RuleFor(r => r.Type).Must(t => typeConditions.Contains(t))
                    .WithMessage($"Type of Prototype must be {typeConditions[0]}, {typeConditions[1]} or empty.");
            }
        }
    }
}