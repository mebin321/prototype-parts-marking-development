namespace WebApi.Data
{
    public class OutletToProductGroupRelation
    {
        public int OutletId { get; set; }

        public Outlet Outlet { get; set; }

        public int ProductGroupId { get; set; }

        public ProductGroup ProductGroup { get; set; }
    }
}