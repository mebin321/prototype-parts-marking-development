namespace WebApi.Data
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class ProductGroupConfiguration : IEntityTypeConfiguration<ProductGroup>
    {
        public void Configure(EntityTypeBuilder<ProductGroup> builder)
        {
            builder.ToTable("ProductGroups");
            builder.HasIndex(e => e.Moniker).IsUnique();
        }
    }
}
