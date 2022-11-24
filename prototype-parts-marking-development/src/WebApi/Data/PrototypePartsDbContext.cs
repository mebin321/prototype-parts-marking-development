namespace WebApi.Data
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using Utilities;
    using WebApi.Common;

    public class PrototypePartsDbContext : DbContext
    {
        private readonly ILoggerFactory loggerFactory;
        private readonly IDateTime dateTime;

        // DbContextOptions are required to be able to use configuration from Startup
        public PrototypePartsDbContext(ILoggerFactory loggerFactory, IDateTime dateTime, DbContextOptions<PrototypePartsDbContext> options)
            : base(options)
        {
            Guard.NotNull(loggerFactory, nameof(loggerFactory));
            Guard.NotNull(dateTime, nameof(dateTime));

            this.loggerFactory = loggerFactory;
            this.dateTime = dateTime;
        }

        public DbSet<User> Users { get; set; }

        public DbSet<Location> Locations { get; set; }

        public DbSet<Outlet> Outlets { get; set; }

        public DbSet<ProductGroup> ProductGroups { get; set; }

        public DbSet<Part> Parts { get; set; }

        public DbSet<GateLevel> GateLevels { get; set; }

        public DbSet<EvidenceYear> EvidenceYears { get; set; }

        public DbSet<PrototypeCounter> PrototypeCounters { get; set; }

        public DbSet<PrototypeSet> PrototypeSets { get; set; }

        public DbSet<Prototype> Prototypes { get; set; }

        public DbSet<PrototypeVariant> PrototypeVariants { get; set; }

        public DbSet<PrototypesPackage> PrototypesPackages { get; set; }

        public DbSet<Role> Roles { get; set; }

        public DbSet<UserRole> UserRoles { get; set; }

        public DbSet<RefreshToken> RefreshTokens { get; set; }

        public DbSet<GlobalProject> GlobalProjects { get; set; }

        public DbSet<PrintingLabel> PrintingLabels { get; set; }

        public DbSet<OutletToProductGroupRelation> OutletToProductGroupRelations { get; set; }

        public DbSet<ProductGroupToPartRelation> ProductGroupToPartRelations { get; set; }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            UpdateTimestamps();

            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            UpdateTimestamps();

            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            optionsBuilder.UseLoggerFactory(loggerFactory);
        }

        // TODO OnDelete behavior
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(PrototypePartsDbContext).Assembly);
            modelBuilder.UseIdentityColumns();
        }

        private void UpdateTimestamps()
        {
            var timestamp = dateTime.Now;

            foreach (var entry in ChangeTracker.Entries().Where(e => e.Entity is IAuditableEntity))
            {
                var auditableEntry = (IAuditableEntity)entry.Entity;

                switch (entry.State)
                {
                    case EntityState.Added:
                        {
                            auditableEntry.CreatedAt = timestamp;
                            auditableEntry.ModifiedAt = timestamp;

                            break;
                        }

                    case EntityState.Modified:
                        {
                            auditableEntry.ModifiedAt = timestamp;

                            break;
                        }

                    case EntityState.Deleted:
                        // Set the entity to unchanged (if we mark the whole entity as Modified, every field gets sent to Db as an update)
                        entry.State = EntityState.Unchanged;
                        auditableEntry.DeletedAt = timestamp;
                        auditableEntry.ModifiedAt = timestamp;

                        break;
                }
            }
        }
    }
}
