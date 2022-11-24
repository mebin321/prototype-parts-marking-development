namespace WebApi.Data
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class PrototypesPackageConfiguration : IEntityTypeConfiguration<PrototypesPackage>
    {
        public void Configure(EntityTypeBuilder<PrototypesPackage> builder)
        {
            builder.ToTable("PrototypesPackages");

            builder.Property(p => p.ModifiedAt).IsConcurrencyToken();
        }
    }
}
