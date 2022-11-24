namespace WebApi.Features.Prototypes.Models
{
    public class UpdatePrototypeDto
    {
        public int Owner { get; set; }

        public string Comment { get; set; }

        public string MaterialNumber { get; set; }

        public string RevisionCode { get; set; }
    }
}
