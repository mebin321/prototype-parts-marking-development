namespace WebApi.Features.AdUsers.Models
{
    using Common.ActiveDirectory;

    public class AdUserDto
    {
        public string Name { get; set; }

        public string Email { get; set; }

        public string Username { get; set; }

        public static AdUserDto From(AdUser entity)
        {
            return new AdUserDto
            {
                Name = entity.Name,
                Email = entity.Email,
                Username = entity.Username,
            };
        }
    }
}
