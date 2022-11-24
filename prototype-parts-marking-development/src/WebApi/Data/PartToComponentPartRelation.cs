namespace WebApi.Data
{
    public class PartToComponentPartRelation
    {
        public int PartId { get; set; }

        public Part Part { get; set; }

        public int ComponentPartId { get; set; }

        public Part ComponentPart { get; set; }
    }
}