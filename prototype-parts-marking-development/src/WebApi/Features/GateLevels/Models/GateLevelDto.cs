namespace WebApi.Features.GateLevels.Models
{
    using WebApi.Data;

    public class GateLevelDto
    {
        public string Moniker { get; set; }

        public string Title { get; set; }

        public string Code { get; set; }

        public string Description { get; set; }

        public static GateLevelDto From(GateLevel entity)
        {
            return new GateLevelDto
            {
                Moniker = entity.Moniker,
                Title = entity.Title,
                Code = entity.Code,
                Description = entity.Description,
            };
        }
    }
}
