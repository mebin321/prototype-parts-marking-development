namespace WebApi.Features.PrototypeVariants.Requests
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
    using Microsoft.EntityFrameworkCore;
    using Models;
    using Swashbuckle.AspNetCore.Annotations;
    using Utilities;

    public class ListPrototypeVariantsQuery : IRequest<PagedDataDto<PrototypeVariantDto>>
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

        public int SetId { get; set; }

        public int PrototypeId { get; set; }

        public class Handler : IRequestHandler<ListPrototypeVariantsQuery, PagedDataDto<PrototypeVariantDto>>
        {
            private readonly IDbContextFactory<PrototypePartsDbContext> dbContextFactory;
            private readonly IProblemDetailsFactory problemDetailsFactory;

            public Handler(IDbContextFactory<PrototypePartsDbContext> dbContextFactory, IProblemDetailsFactory problemDetailsFactory)
            {
                Guard.NotNull(dbContextFactory, nameof(dbContextFactory));
                Guard.NotNull(problemDetailsFactory, nameof(problemDetailsFactory));

                this.dbContextFactory = dbContextFactory;
                this.problemDetailsFactory = problemDetailsFactory;
            }

            public async Task<PagedDataDto<PrototypeVariantDto>> Handle(
                ListPrototypeVariantsQuery request,
                CancellationToken cancellationToken)
            {
                await using var dbContext = dbContextFactory.CreateDbContext();

                var set = await dbContext.PrototypeSets
                    .Include(s => s.Prototypes.Where(p => p.Id == request.PrototypeId))
                    .FirstOrDefaultAsync(s => s.Id == request.SetId, CancellationToken.None)
                    .ThrowIfNullAsync(problemDetailsFactory.EntityNotFound(nameof(PrototypeSet), request.SetId));

                if (set.Prototypes.Count == 0)
                {
                    throw problemDetailsFactory.EntityNotFound(nameof(Prototype), request.PrototypeId);
                }

                var query = dbContext.PrototypeVariants
                    .AsNoTracking()
                    .Include(v => v.CreatedBy)
                    .Include(v => v.ModifiedBy)
                    .Include(v => v.DeletedBy)
                    .Where(v => v.PrototypeId == request.PrototypeId)
                    .OrderByDescending(v => v.Version)
                    .AsQueryable();
                var prototypes = await PagedList.CreateAsync(query, request.Page, request.PageSize);

                return new PagedDataDto<PrototypeVariantDto>
                {
                    Items = prototypes.Select(p => PrototypeVariantDto.From(p)).ToList(),
                    Pagination = prototypes.CreatePagination(),
                };
            }
        }

        public class Validator : AbstractValidator<ListPrototypeVariantsQuery>
        {
            public Validator()
            {
                RuleFor(r => r.SetId).GreaterThan(0);
                RuleFor(r => r.PrototypeId).GreaterThan(0);
            }
        }
    }
}