namespace WebApi.Features.Roles.Models
{
    using System.ComponentModel.DataAnnotations;

    public class UpdateRoleRequestDto
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }
    }
}