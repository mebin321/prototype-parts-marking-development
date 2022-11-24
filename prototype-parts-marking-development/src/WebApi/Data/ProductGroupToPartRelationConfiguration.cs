namespace WebApi.Data
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class ProductGroupToPartRelationConfiguration : IEntityTypeConfiguration<ProductGroupToPartRelation>
    {
        public void Configure(EntityTypeBuilder<ProductGroupToPartRelation> builder)
        {
            builder.ToTable("ProductGroupPartRelations");
            builder.HasKey(e => new
            {
                e.ProductGroupId,
                e.PartId,
            });
        }
    }
}