namespace DataMigrator
{
    public class DataModel
    {
        public string Customer { get; set; }
        public string ProjectNumber { get; set; }
        public string Comment { get; set; }
        public string CreatedBy { get; set; }
        public string Outlet { get; set; }
        public string OutletCode { get; set; }
        public string ProductGroup { get; set; }
        public string ProductGroupCode { get; set; }
        public string Part { get; set; }
        public string PartCode { get; set; }
        public string SetIdentifier { get; set; }
        public string EvidenceYearCode { get; set; }
        public string Location { get; set; }
        public string LocationCode { get; set; }
        public string GateLevelCode { get; set; }
        public int Index { get; set; }
        public int IsActive { get; set; }

        public override string ToString()
        {
            return $"{Customer} {ProjectNumber} {Comment} {CreatedBy} {Outlet} {OutletCode} {ProductGroup} {ProductGroupCode} {Part} {PartCode} {SetIdentifier} {EvidenceYearCode} {Location} {LocationCode} {GateLevelCode} {Index} {IsActive} ";
        }
    }


}