namespace WebApi.Features.Parts.Models
{
    using System.ComponentModel.DataAnnotations;

    public class UpdatePartRequestDto
    {
        [Required]
        public string Description { get; set; }
    }
}
