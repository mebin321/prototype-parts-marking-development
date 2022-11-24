namespace WebApi.Data
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class PrototypeVariantConfiguration : IEntityTypeConfiguration<PrototypeVariant>
    {
        public void Configure(EntityTypeBuilder<PrototypeVariant> builder)
        {
            builder.ToTable("PrototypeVariants");

            builder.Property(v => v.ModifiedAt).IsConcurrencyToken();
        }
    }
}
