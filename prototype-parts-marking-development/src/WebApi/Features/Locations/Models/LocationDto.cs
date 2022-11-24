namespace WebApi.Features.Locations.Models
{
    using System.ComponentModel.DataAnnotations;
    using WebApi.Data;

    public class LocationDto
    {
        [Required]
        public string Moniker { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        [MaxLength(2)]
        [MinLength(2)]
        public string Code { get; set; }

        [Required]
        public string Description { get; set; }

        public static LocationDto From(Location entity)
        {
            return new LocationDto
            {
                Moniker = entity.Moniker,
                Title = entity.Title,
                Code = entity.Code,
                Description = entity.Description,
            };
        }
    }
}
