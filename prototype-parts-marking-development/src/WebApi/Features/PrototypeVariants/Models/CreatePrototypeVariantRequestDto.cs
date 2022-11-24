namespace WebApi.Features.PrototypeVariants.Models
{
    using System.ComponentModel.DataAnnotations;

    public class CreatePrototypeVariantRequestDto
    {
        [Required]
        public string Comment { get; set; }
    }
}
