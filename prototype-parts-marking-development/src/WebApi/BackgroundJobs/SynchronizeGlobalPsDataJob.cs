namespace WebApi.BackgroundJobs
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Utilities;
    using WebApi.Data;
    using WebApi.Features.GlobalProjects.Services;

    public class SynchronizeGlobalPsDataJob
    {
        private readonly IDbContextFactory<PrototypePartsDbContext> dbContextFactory;
        private readonly ICobraRepository cobraRepository;

        public SynchronizeGlobalPsDataJob(IDbContextFactory<PrototypePartsDbContext> dbContextFactory, ICobraRepository cobraRepository)
        {
            Guard.NotNull(dbContextFactory, nameof(dbContextFactory));
            Guard.NotNull(cobraRepository, nameof(cobraRepository));

            this.dbContextFactory = dbContextFactory;
            this.cobraRepository = cobraRepository;
        }

        public async Task Execute(CancellationToken cancellationToken)
        {
            await using var context = dbContextFactory.CreateDbContext();
            var dbProjects = await context.GlobalProjects.ToDictionaryAsync(p => p.Id, cancellationToken);

            await foreach (var project in cobraRepository.GetGlobalProjects(cancellationToken))
            {
                if (dbProjects.TryGetValue(project.ProjectPk, out var existing))
                {
                    existing.Description = project.ProjectText;
                    existing.ProjectNumber = project.ProjectNumber;
                    existing.Customer = project.Manufacturer;
                }
                else
                {
                    context.GlobalProjects.Add(new GlobalProject
                    {
                        Id = project.ProjectPk,
                        Description = project.ProjectText,
                        ProjectNumber = project.ProjectNumber,
                        Customer = project.Manufacturer,
                    });
                }
            }

            await context.SaveChangesAsync();
        }
    }
}