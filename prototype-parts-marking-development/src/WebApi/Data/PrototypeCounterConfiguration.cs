namespace WebApi.Data
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class PrototypeCounterConfiguration : IEntityTypeConfiguration<PrototypeCounter>
    {
        public void Configure(EntityTypeBuilder<PrototypeCounter> builder)
        {
            builder.ToTable("PrototypeCounters");
            builder.HasIndex(p => new { p.LocationId, p.EvidenceYearId }).IsUnique();

            builder.HasOne(p => p.Location)
                .WithMany(o => o.PrototypeCounters)
                .HasForeignKey(p => p.LocationId);

            builder.HasOne(p => p.EvidenceYear)
                .WithMany(o => o.PrototypeCounters)
                .HasForeignKey(p => p.EvidenceYearId);
        }
    }
}