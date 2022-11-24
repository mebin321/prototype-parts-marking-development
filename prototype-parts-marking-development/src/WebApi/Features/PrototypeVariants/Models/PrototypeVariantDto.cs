namespace WebApi.Features.PrototypeVariants.Models
{
    using System;
    using Data;
    using Users.Models;

    public class PrototypeVariantDto
    {
        public int Id { get; set; }

        public int PrototypeId { get; set; }

        public int Version { get; set; }

        public string Comment { get; set; }

        public DateTimeOffset? CreatedAt { get; set; }

        public UserDto CreatedBy { get; set; }

        public DateTimeOffset? ModifiedAt { get; set; }

        public UserDto ModifiedBy { get; set; }

        public DateTimeOffset? DeletedAt { get; set; }

        public UserDto DeletedBy { get; set; }

        public static PrototypeVariantDto From(PrototypeVariant entity)
        {
            return new PrototypeVariantDto
            {
                Id = entity.Id,
                PrototypeId = entity.PrototypeId,
                Version = entity.Version,
                Comment = entity.Comment,
                CreatedAt = entity.CreatedAt,
                CreatedBy = UserDto.From(entity.CreatedBy),
                ModifiedAt = entity.ModifiedAt,
                ModifiedBy = UserDto.From(entity.ModifiedBy),
                DeletedAt = entity.DeletedAt,
                DeletedBy = UserDto.From(entity.DeletedBy),
            };
        }
    }
}