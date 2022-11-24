namespace WebApi.Data
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class GateLevelConfiguration : IEntityTypeConfiguration<GateLevel>
    {
        public void Configure(EntityTypeBuilder<GateLevel> builder)
        {
            builder.ToTable("GateLevels");
            builder.HasIndex(e => e.Moniker).IsUnique();
        }
    }
}