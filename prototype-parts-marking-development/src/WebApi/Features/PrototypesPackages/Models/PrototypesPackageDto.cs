namespace WebApi.Features.PrototypesPackages.Models
{
    using System;
    using Data;
    using Users.Models;

    public class PrototypesPackageDto
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

        public string Customer { get; set; }

        public string Project { get; set; }

        public string ProjectNumber { get; set; }

        public UserDto Owner { get; set; }

        public string Comment { get; set; }

        public DateTimeOffset? CreatedAt { get; set; }

        public UserDto CreatedBy { get; set; }

        public DateTimeOffset? ModifiedAt { get; set; }

        public UserDto ModifiedBy { get; set; }

        public DateTimeOffset? DeletedAt { get; set; }

        public UserDto DeletedBy { get; set; }

        public static PrototypesPackageDto From(PrototypesPackage entity)
        {
            return new PrototypesPackageDto
            {
                Id = entity.Id,
                OutletCode = entity.OutletCode,
                OutletTitle = entity.OutletTitle,
                ProductGroupCode = entity.ProductGroupCode,
                ProductGroupTitle = entity.ProductGroupTitle,
                GateLevelCode = entity.GateLevelCode,
                GateLevelTitle = entity.GateLevelTitle,
                EvidenceYearCode = entity.EvidenceYearCode,
                EvidenceYearTitle = entity.EvidenceYearTitle,
                LocationCode = entity.LocationCode,
                LocationTitle = entity.LocationTitle,
                PartTypeCode = entity.PartTypeCode,
                PartTypeTitle = entity.PartTypeTitle,
                PackageIdentifier = entity.PackageIdentifier,
                InitialCount = entity.InitialCount,
                ActualCount = entity.ActualCount,
                Owner = UserDto.From(entity.Owner),
                Comment = entity.Comment,
                CreatedAt = entity.CreatedAt,
                CreatedBy = UserDto.From(entity.CreatedBy),
                ModifiedAt = entity.ModifiedAt,
                ModifiedBy = UserDto.From(entity.ModifiedBy),
                DeletedAt = entity.DeletedAt,
                DeletedBy = UserDto.From(entity.DeletedBy),
                Customer = entity.Customer,
                Project = entity.Project,
                ProjectNumber = entity.ProjectNumber,
            };
        }
    }
}