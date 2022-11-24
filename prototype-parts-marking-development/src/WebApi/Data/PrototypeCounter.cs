namespace WebApi.Data
{
    public class PrototypeCounter
    {
        public int Id { get; set; }

        public int LocationId { get; set; }

        public Location Location { get; set; }

        public int EvidenceYearId { get; set; }

        public EvidenceYear EvidenceYear { get; set; }

        public int Value { get; set; }
    }
}
