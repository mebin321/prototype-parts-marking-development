namespace WebApi.Features.Users.Models
{
    using System;
    using WebApi.Data;

    public class EnrichedUserDto
    {
        public int? Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string Username { get; set; }

        public DateTimeOffset? CreatedAt { get; set; }

        public DateTimeOffset? ModifiedAt { get; set; }

        public DateTimeOffset? DeletedAt { get; set; }

        public static EnrichedUserDto From(User entity)
        {
            if (entity == null)
            {
                return new EnrichedUserDto();
            }

            return new EnrichedUserDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Email = entity.Email,
                Username = entity.DomainIdentity,
                CreatedAt = entity.CreatedAt,
                ModifiedAt = entity.ModifiedAt,
                DeletedAt = entity.DeletedAt,
            };
        }
    }
}
