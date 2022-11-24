namespace WebApi.Data
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class OutletToProductGroupRelationConfiguration : IEntityTypeConfiguration<OutletToProductGroupRelation>
    {
        public void Configure(EntityTypeBuilder<OutletToProductGroupRelation> builder)
        {
            builder.ToTable("OutletToProductGroupRelations");
            builder.HasKey(e => new
            {
                e.OutletId,
                e.ProductGroupId,
            });
        }
    }
}