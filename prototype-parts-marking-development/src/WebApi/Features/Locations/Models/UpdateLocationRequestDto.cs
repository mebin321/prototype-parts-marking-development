namespace WebApi.Features.Locations.Models
{
    using System.ComponentModel.DataAnnotations;

    public class UpdateLocationRequestDto
    {
        [Required]
        public string Description { get; set; }
    }
}