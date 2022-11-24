namespace WebApi.Common.PrototypeIdentifier
{
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Utilities;
    using WebApi.Data;

    public class PrototypeIdentifierCounter : IPrototypeIdentifierCounter
    {
        private readonly IDbContextFactory<PrototypePartsDbContext> dbContextFactory;

        public PrototypeIdentifierCounter(IDbContextFactory<PrototypePartsDbContext> dbContextFactory)
        {
            Guard.NotNull(dbContextFactory, nameof(dbContextFactory));

            this.dbContextFactory = dbContextFactory;
        }

        public async Task<int> IncrementCounterFor(int locationId, int evidenceYearId)
        {
            await using var dbContext = dbContextFactory.CreateDbContext();

            var counter = await dbContext.PrototypeCounters
                .FirstOrDefaultAsync(c => c.LocationId == locationId && c.EvidenceYearId == evidenceYearId);

            if (counter == null)
            {
                counter = new PrototypeCounter
                {
                    LocationId = locationId,
                    EvidenceYearId = evidenceYearId,
                    Value = 0,
                };
                dbContext.PrototypeCounters.Add(counter);
            }
            else
            {
                counter.Value++;
            }

            await dbContext.SaveChangesAsync();

            return counter.Value;
        }
    }
}