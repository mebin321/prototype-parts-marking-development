namespace WebApi.Features.ProductGroup.Models
{
    using WebApi.Data;

    public class ProductGroupDto
    {
        public string Moniker { get; set; }

        public string Title { get; set; }

        public string Code { get; set; }

        public string Description { get; set; }

        public static ProductGroupDto From(ProductGroup entity)
        {
            return new ProductGroupDto
            {
                Moniker = entity.Moniker,
                Title = entity.Title,
                Code = entity.Code,
                Description = entity.Description,
            };
        }
    }
}
