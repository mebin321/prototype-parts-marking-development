namespace WebApi.Features.GlobalProjects.Requests
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Threading.Tasks;
    using FluentValidation;
    using MediatR;
    using Microsoft.EntityFrameworkCore;
    using Utilities;
    using WebApi.Common;
    using WebApi.Data;
    using WebApi.Features.GlobalProjects.Models;

    public class GetGlobalProjectsQuery : IRequest<GlobalProjectDto[]>
    {
        private const int MaxLimit = 100;

        private int limit = 100;

        public string ProjectNumber { get; set; }

        public string Customer { get; set; }

        public string Search { get; set; }

        [Range(1, MaxLimit)]
        public int Limit
        {
            get => limit;
            set => limit = Math.Clamp(value, 1, MaxLimit);
        }

        public class Handler : IRequestHandler<GetGlobalProjectsQuery, GlobalProjectDto[]>
        {
            private readonly IDbContextFactory<PrototypePartsDbContext> dbContextFactory;

            public Handler(IDbContextFactory<PrototypePartsDbContext> dbContextFactory)
            {
                Guard.NotNull(dbContextFactory, nameof(dbContextFactory));

                this.dbContextFactory = dbContextFactory;
            }

            public async Task<GlobalProjectDto[]> Handle(GetGlobalProjectsQuery request, CancellationToken cancellationToken)
            {
                await using var context = dbContextFactory.CreateDbContext();

                var query = context.GlobalProjects
                    .AsNoTracking()
                    .FilterWith(request.ProjectNumber, nameof(GlobalProject.ProjectNumber))
                    .FilterWith(request.Customer, nameof(GlobalProject.Customer));

                if (!string.IsNullOrWhiteSpace(request.Search))
                {
                    query = query.Where(p
                        => p.SearchVector.Matches(EF.Functions.ToTsQuery("english", TextSearchQueryFrom(request.Search))));
                }

                query = query
                    .OrderBy(p => p.Id)
                    .Take(request.Limit);

                var projects = await query.ToListAsync(CancellationToken.None);
                return projects
                    .Distinct(new ProjectNumberComparer())
                    .Select(p => new GlobalProjectDto
                    {
                        ProjectNumber = p.ProjectNumber,
                        Description = p.Description,
                        Customer = p.Customer,
                    })
                    .ToArray();
            }

            private static string TextSearchQueryFrom(string search)
            {
                var split = search
                    .ToLowerInvariant()
                    .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

                // https://www.postgresql.org/docs/current/datatype-textsearch.html
                return $"{string.Join(" & ", split)}:*";
            }
        }

        public class Validator : AbstractValidator<GetGlobalProjectsQuery>
        {
            private static readonly Regex NoSpecialCharacters = new(@"^[\w\-\ ]*$");

            public Validator()
            {
                RuleFor(r => r.Search)
                    .Matches(NoSpecialCharacters)
                    .WithMessage("Search expression may not contain special characters.");
            }
        }
    }
}