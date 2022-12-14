namespace WebApi.Features.Prototypes.Models
{
    using System;
    using Data;
    using Users.Models;

    public class PrototypeDto
    {
        public int Id { get; set; }

        public int PrototypeSetId { get; set; }

        public string PartTypeCode { get; set; }

        public string PartTypeTitle { get; set; }

        public string Type { get; set; }

        public int Index { get; set; }

        public string MaterialNumber { get; set; }

        public string RevisionCode { get; set; }

        public UserDto Owner { get; set; }

        public string Comment { get; set; }

        public DateTimeOffset? CreatedAt { get; set; }

        public UserDto CreatedBy { get; set; }

        public DateTimeOffset? ModifiedAt { get; set; }

        public UserDto ModifiedBy { get; set; }

        public DateTimeOffset? DeletedAt { get; set; }

        public UserDto DeletedBy { get; set; }

        public static PrototypeDto From(Prototype entity)
        {
            return new PrototypeDto
            {
                Id = entity.Id,
                PrototypeSetId = entity.PrototypeSetId,
                PartTypeCode = entity.PartTypeCode,
                PartTypeTitle = entity.PartTypeTitle,
                Type = entity.Type.ToString(),
                Index = entity.Index,
                MaterialNumber = entity.MaterialNumber,
                RevisionCode = entity.RevisionCode,
                Owner = UserDto.From(entity.Owner),
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