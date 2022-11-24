namespace WebApi.Data
{
    using System.Collections.Generic;

    public class ProductGroup
    {
        public ProductGroup()
        {
            OutletToProductGroupRelations = new List<OutletToProductGroupRelation>();
            ProductGroupToPartRelations = new List<ProductGroupToPartRelation>();
        }

        public int Id { get; set; }

        public string Moniker { get; set; }

        public string Title { get; set; }

        public string Code { get; set; }

        public string Description { get; set; }

        public List<OutletToProductGroupRelation> OutletToProductGroupRelations { get; }

        public List<ProductGroupToPartRelation> ProductGroupToPartRelations { get; }
    }
}
