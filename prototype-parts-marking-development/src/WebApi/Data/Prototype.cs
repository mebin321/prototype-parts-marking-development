namespace WebApi.Data
{
    using System;
    using System.Collections.Generic;

    public class Prototype : IAuditableEntity
    {
        public int Id { get; set; }

        public int PrototypeSetId { get; set; }

        public PrototypeSet PrototypeSet { get; set; }

        public string PartTypeCode { get; set; }

        public string PartTypeTitle { get; set; }

        public PrototypeType Type { get; set; }

        public int Index { get; set; }

        public string Comment { get; set; }

        public string MaterialNumber { get; set; }

        public string RevisionCode { get; set; }

        public int OwnerId { get; set; }

        public User Owner { get; set; }

        public List<PrototypeVariant> PrototypeVariants { get; set; }

        public DateTimeOffset? CreatedAt { get; set; }

        public int CreatedById { get; set; }

        public User CreatedBy { get; set; }

        public DateTimeOffset? ModifiedAt { get; set; }

        public int ModifiedById { get; set; }

        public User ModifiedBy { get; set; }

        public DateTimeOffset? DeletedAt { get; set; }

        public int? DeletedById { get; set; }

        public User DeletedBy { get; set; }
    }
}
