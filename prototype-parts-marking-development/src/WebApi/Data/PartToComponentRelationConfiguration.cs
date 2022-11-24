namespace WebApi.Data
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class PartToComponentRelationConfiguration : IEntityTypeConfiguration<PartToComponentPartRelation>
    {
        public void Configure(EntityTypeBuilder<PartToComponentPartRelation> builder)
        {
            builder.ToTable("PartToComponentPartRelations");
            builder.HasKey(e => new
            {
                e.PartId,
                e.ComponentPartId,
            });

            builder
                .HasOne(p => p.Part)
                .WithMany(p => p.PartToComponentRelations)
                .HasForeignKey(p => p.PartId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}