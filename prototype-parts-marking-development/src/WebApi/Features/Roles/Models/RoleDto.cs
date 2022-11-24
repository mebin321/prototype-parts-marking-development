namespace WebApi.Features.Roles.Models
{
    using WebApi.Data;

    public class RoleDto
    {
        public string Moniker { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public static RoleDto From(Role entity)
        {
            return new RoleDto
            {
                Moniker = entity.Moniker,
                Title = entity.Title,
                Description = entity.Description,
            };
        }
    }
}
