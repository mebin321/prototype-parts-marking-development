namespace WebApi.Data
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class OutletConfiguration : IEntityTypeConfiguration<Outlet>
    {
        public void Configure(EntityTypeBuilder<Outlet> builder)
        {
            builder.ToTable("Outlets");
            builder.HasIndex(e => e.Moniker).IsUnique();
        }
    }
}
