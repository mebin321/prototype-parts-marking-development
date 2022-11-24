namespace WebApi.Data
{
    public class ProductGroupToPartRelation
    {
        public int ProductGroupId { get; set; }

        public ProductGroup ProductGroup { get; set; }

        public int PartId { get; set; }

        public Part Part { get; set; }
    }
}