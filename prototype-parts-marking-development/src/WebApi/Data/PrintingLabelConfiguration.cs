namespace WebApi.Data
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class PrintingLabelConfiguration : IEntityTypeConfiguration<PrintingLabel>
    {
        public void Configure(EntityTypeBuilder<PrintingLabel> builder)
        {
            builder.ToTable("PrintingLabels");
        }
    }
}