namespace WebApi.Data
{
    using System;

    public class PrototypeVariant : IAuditableEntity
    {
        public int Id { get; set; }

        public int PrototypeId { get; set; }

        public Prototype Prototype { get; set; }

        public int Version { get; set; }

        public string Comment { get; set; }

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
