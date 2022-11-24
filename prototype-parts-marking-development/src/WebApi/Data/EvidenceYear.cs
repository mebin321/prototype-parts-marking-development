namespace WebApi.Data
{
    using System.Collections.Generic;

    public class EvidenceYear
    {
        public int Id { get; set; }

        public int Year { get; set; }

        public string Code { get; set; }

        public List<PrototypeCounter> PrototypeCounters { get; set; }
    }
}