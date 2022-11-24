namespace WebApi.Data
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class GlobalProjectConfiguration : IEntityTypeConfiguration<GlobalProject>
    {
        public void Configure(EntityTypeBuilder<GlobalProject> builder)
        {
            builder.ToTable("GlobalProjects");
            builder.Property(e => e.Id).ValueGeneratedNever();

            builder.HasIndex(p => p.ProjectNumber);
            builder.HasIndex(p => p.Customer);

            builder.HasGeneratedTsVectorColumn(
                    p => p.SearchVector,
                    "english",
                    p => new { p.ProjectNumber, p.Description, p.Customer })
                .HasIndex(p => p.SearchVector)
                .HasMethod("GIN"); // https://www.postgresql.org/docs/current/textsearch-indexes.html
        }
    }
}