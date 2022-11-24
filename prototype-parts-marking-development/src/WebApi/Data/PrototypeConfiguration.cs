namespace WebApi.Data
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class PrototypeConfiguration : IEntityTypeConfiguration<Prototype>
    {
        public void Configure(EntityTypeBuilder<Prototype> builder)
        {
            builder.ToTable("Prototypes");

            builder.HasOne(p => p.PrototypeSet)
                .WithMany(o => o.Prototypes)
                .HasForeignKey(p => p.PrototypeSetId);

            builder.Property(p => p.Type).HasConversion<string>();

            builder.Property(p => p.ModifiedAt).IsConcurrencyToken();
        }
    }
}
