namespace WebApi.Data
{
    using System.Collections.Generic;

    public class Location
    {
        public int Id { get; set; }

        public string Moniker { get; set; }

        public string Title { get; set; }

        public string Code { get; set; }

        public string Description { get; set; }

        public List<PrototypeCounter> PrototypeCounters { get; set; }
    }
}