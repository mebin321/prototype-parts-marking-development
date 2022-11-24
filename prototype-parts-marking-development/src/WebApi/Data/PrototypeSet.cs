namespace WebApi.Data
{
    using System;
    using System.Collections.Generic;

    public class PrototypeSet : IAuditableEntity
    {
        public int Id { get; set; }

        public string OutletCode { get; set; }

        public string OutletTitle { get; set; }

        public string ProductGroupCode { get; set; }

        public string ProductGroupTitle { get; set; }

        public string GateLevelCode { get; set; }

        public string GateLevelTitle { get; set; }

        public string EvidenceYearCode { get; set; }

        public int EvidenceYearTitle { get; set; }

        public string LocationCode { get; set; }

        public string LocationTitle { get; set; }

        public string SetIdentifier { get; set; }

        public string Customer { get; set; }

        public string Project { get; set; }

        public string ProjectNumber { get; set; }

        public List<Prototype> Prototypes { get; set; }

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