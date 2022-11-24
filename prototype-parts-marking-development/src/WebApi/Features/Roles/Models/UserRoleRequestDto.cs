namespace WebApi.Features.Roles.Models
{
    using System.ComponentModel.DataAnnotations;

    public class UserRoleRequestDto
    {
        [Required]
        public string Moniker { get; set; }
    }
}