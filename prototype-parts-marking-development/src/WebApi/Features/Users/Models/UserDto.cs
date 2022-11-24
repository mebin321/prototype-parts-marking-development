namespace WebApi.Features.Users.Models
{
    using WebApi.Data;

    public class UserDto
    {
        public int? Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string Username { get; set; }

        public static UserDto From(User entity)
        {
            if (entity == null)
            {
                return new UserDto();
            }

            return new UserDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Email = entity.Email,
                Username = entity.DomainIdentity,
            };
        }
    }
}
