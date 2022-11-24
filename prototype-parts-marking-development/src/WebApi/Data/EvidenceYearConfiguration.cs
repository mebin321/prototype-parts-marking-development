namespace WebApi.Data
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class EvidenceYearConfiguration : IEntityTypeConfiguration<EvidenceYear>
    {
        public void Configure(EntityTypeBuilder<EvidenceYear> builder)
        {
            builder.ToTable("EvidenceYears");
            builder.HasIndex(e => e.Year).IsUnique();
        }
    }
}