namespace WebApi.Features.EvidenceYears.Models
{
    using Data;

    public class EvidenceYearDto
    {
        public int Year { get; set; }

        public string Code { get; set; }

        public static EvidenceYearDto From(EvidenceYear entity)
        {
            return new EvidenceYearDto
            {
                Year = entity.Year,
                Code = entity.Code,
            };
        }
    }
}