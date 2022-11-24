namespace WebApi.Data
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class PrototypeSetConfiguration : IEntityTypeConfiguration<PrototypeSet>
    {
        public void Configure(EntityTypeBuilder<PrototypeSet> builder)
        {
            builder.ToTable("PrototypeSets");

            builder.Property(p => p.ModifiedAt).IsConcurrencyToken();
        }
    }
}