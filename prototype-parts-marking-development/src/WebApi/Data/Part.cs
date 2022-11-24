namespace WebApi.Data
{
    using System.Collections.Generic;

    public class Part
    {
        public Part()
        {
            ProductGroupToPartRelations = new List<ProductGroupToPartRelation>();
            PartToComponentRelations = new List<PartToComponentPartRelation>();
        }

        public int Id { get; set; }

        public string Moniker { get; set; }

        public string Title { get; set; }

        public string Code { get; set; }

        public string Description { get; set; }

        public List<ProductGroupToPartRelation> ProductGroupToPartRelations { get; }

        public List<PartToComponentPartRelation> PartToComponentRelations { get; }
    }
}
