namespace WebApi.Features.PrintingLabels.Models
{
    using System;
    using Data;

    public class PrintingLabelDto
    {
        public int Id { get; set; }

        public int OwnerId { get; set; }

        public string Customer { get; set; }

        public string ProductGroup { get; set; }

        public string Outlet { get; set; }

        public string Location { get; set; }

        public string ProjectNumber { get; set; }

        public string GateLevel { get; set; }

        public string MaterialNumber { get; set; }

        public string RevisionCode { get; set; }

        public string Description { get; set; }

        public string PartType { get; set; }

        public string PartCode { get; set; }

        public DateTimeOffset? CreatedAt { get; set; }

        public static PrintingLabelDto From(PrintingLabel entity)
        {
            return new PrintingLabelDto
            {
                Id = entity.Id,
                OwnerId = entity.OwnerId,
                Customer = entity.Customer,
                ProductGroup = entity.ProductGroup,
                Outlet = entity.Outlet,
                Location = entity.Location,
                ProjectNumber = entity.ProjectNumber,
                GateLevel = entity.GateLevel,
                MaterialNumber = entity.MaterialNumber,
                RevisionCode = entity.RevisionCode,
                Description = entity.Description,
                PartType = entity.PartType,
                PartCode = entity.PartCode,
                CreatedAt = entity.CreatedAt,
            };
        }
    }
}
