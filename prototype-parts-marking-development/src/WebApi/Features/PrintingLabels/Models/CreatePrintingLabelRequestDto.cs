namespace WebApi.Features.PrintingLabels.Models
{
    public class CreatePrintingLabelRequestDto
    {
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
    }
}
