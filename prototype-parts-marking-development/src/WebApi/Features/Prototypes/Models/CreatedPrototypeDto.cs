namespace WebApi.Features.Prototypes.Models
{
    using Data;
    using Users.Models;

    public class CreatedPrototypeDto : PrototypeDto
    {
        public string Href { get; set; }

        public static CreatedPrototypeDto From(Prototype entity, string href)
        {
            return new CreatedPrototypeDto
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
                Href = href,
            };
        }
    }
}
