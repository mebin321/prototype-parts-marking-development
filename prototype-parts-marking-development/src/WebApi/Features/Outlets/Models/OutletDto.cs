namespace WebApi.Features.Outlets.Models
{
    using WebApi.Data;

    public class OutletDto
    {
        public string Moniker { get; set; }

        public string Title { get; set; }

        public string Code { get; set; }

        public string Description { get; set; }

        public static OutletDto From(Outlet entity)
        {
            return new OutletDto
            {
                Moniker = entity.Moniker,
                Title = entity.Title,
                Code = entity.Code,
                Description = entity.Description,
            };
        }
    }
}
