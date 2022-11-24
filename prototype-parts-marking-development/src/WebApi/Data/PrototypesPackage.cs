namespace WebApi.Data
{
    using System;

    public class PrototypesPackage : IAuditableEntity
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

        public string PartTypeCode { get; set; }

        public string PartTypeTitle { get; set; }

        public string PackageIdentifier { get; set; }

        public int InitialCount { get; set; }

        public int ActualCount { get; set; }

        public string Comment { get; set; }

        public string Customer { get; set; }

        public string Project { get; set; }

        public string ProjectNumber { get; set; }

        public int OwnerId { get; set; }

        public User Owner { get; set; }

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
