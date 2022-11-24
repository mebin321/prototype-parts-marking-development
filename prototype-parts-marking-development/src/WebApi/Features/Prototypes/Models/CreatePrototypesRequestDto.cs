namespace WebApi.Features.Prototypes.Models
{
    using System.ComponentModel.DataAnnotations;

    public class CreatePrototypesRequestDto
    {
        [Required]
        public int Index { get; set; }

        [Required]
        public string PartMoniker { get; set; }

        [Required]
        [MinLength(13)]
        [MaxLength(13)]
        public string MaterialNumber { get; set; }

        [Required]
        [MinLength(2)]
        [MaxLength(2)]
        public string RevisionCode { get; set; }

        [Required]
        public int OwnerId { get; set; }

        public string Comment { get; set; }
    }
}
