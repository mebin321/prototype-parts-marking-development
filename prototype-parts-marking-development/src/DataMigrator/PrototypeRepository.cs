namespace DataMigrator
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Utilities;
    using WebApi;
    using WebApi.Data;

    public class PrototypeRepository
    {
        private PrototypePartsDbContext dbContext;

        public PrototypeRepository(PrototypePartsDbContext dbContext)
        {
            Guard.NotNull(dbContext,nameof(dbContext));
            this.dbContext = dbContext;
        }

        public async Task<PrototypeSet> GetPrototypeSetAsync(string locationCode, string evidenceYearCode, string setIdentifier)
        {
            return  await dbContext.PrototypeSets
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    s => s.LocationCode == locationCode && s.EvidenceYearCode == evidenceYearCode &&
                         s.SetIdentifier == setIdentifier);
        }

        public async Task<User> FindUserAsync(string userName)
        {
            return await dbContext.Users
                .SingleOrDefaultAsync(u => u.Name == userName)
                .ThrowIfNullAsync(new EntityException($"User with Name:{userName} not found."));
        }

        public async Task<EvidenceYear> FindEvidenceYearAsync(string evidenceYearCode)
        {
            return await dbContext.EvidenceYears
                .SingleOrDefaultAsync(y => y.Code == evidenceYearCode)
                .ThrowIfNullAsync(new EntityException($"EvidenceYear with Code:{evidenceYearCode} not found."));
        }

        public async Task<GateLevel> FindGateLevelAsync(string gateLevelCode)
        {
            return await dbContext.GateLevels
                .SingleOrDefaultAsync(l => l.Code == gateLevelCode)
                .ThrowIfNullAsync(new EntityException($"GateLevel with Code:{gateLevelCode} not found."));
        }

        public async Task<Location> FindLocationAsync(string locationCode, string locationTitle)
        {
            return await dbContext.Locations
                .SingleOrDefaultAsync(l => l.Code == locationCode && l.Title == locationTitle)
                .ThrowIfNullAsync(new EntityException($"Location with Code:{locationCode} and Title:{locationTitle} not found."));
        }
        public async Task<Outlet> FindOutletAsync(string outletCode, string outletTitle)
        {
            return await dbContext.Outlets
                .SingleOrDefaultAsync(o => o.Code == outletCode && o.Title == outletTitle)
                .ThrowIfNullAsync(new EntityException($"Outlet with Code:{outletCode} and Title:{outletTitle} not found."));
        }
        public async Task<ProductGroup> FindProductGroupAsync(string productGroupCode, string productGroupTitle)
        {
            return await dbContext.ProductGroups
                .SingleOrDefaultAsync(g => g.Code == productGroupCode && g.Title == productGroupTitle)
                .ThrowIfNullAsync(new EntityException($"ProductGroup with Code:{productGroupCode} and Title:{productGroupTitle} not found."));
        }
        
        public async Task<Part> FindPartAsync(string partCode, string partTitle)
        {
            return await dbContext.Parts
                .SingleOrDefaultAsync(p => p.Code == partCode && p.Title == partTitle)
                .ThrowIfNullAsync(new EntityException($"Part with Code:{partCode} and Title:{partTitle} not found."));
        }

        public async Task AddPrototypeSetsAsync(IEnumerable<PrototypeSet> sets)
        {
            await dbContext.PrototypeSets.AddRangeAsync(sets);
        }

        public async Task SaveAsync()
        {
            await dbContext.SaveChangesAsync();
        }
    }
}
