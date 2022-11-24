namespace WebApi.BackgroundJobs
{
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Utilities;
    using WebApi.Common;
    using WebApi.Data;

    public class RemoveExpiredRefreshTokensJob
    {
        private readonly IDbContextFactory<PrototypePartsDbContext> dbContextFactory;
        private readonly IDateTime dateTime;

        public RemoveExpiredRefreshTokensJob(IDbContextFactory<PrototypePartsDbContext> dbContextFactory, IDateTime dateTime)
        {
            Guard.NotNull(dbContextFactory, nameof(dbContextFactory));
            Guard.NotNull(dateTime, nameof(dateTime));

            this.dbContextFactory = dbContextFactory;
            this.dateTime = dateTime;
        }

        public async Task Execute()
        {
            await using var dbContext = dbContextFactory.CreateDbContext();

            var expiredTokens = await dbContext.RefreshTokens
                .Where(t => dateTime.Now >= t.ExpiresAt)
                .ToListAsync();

            dbContext.RefreshTokens.RemoveRange(expiredTokens);

            await dbContext.SaveChangesAsync();
        }
    }
}
