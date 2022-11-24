namespace WebApi.Features.PrototypesPackages.Models
{
    public class UpdatePrototypesPackageDto
    {
        public int Owner { get; set; }

        public string Comment { get; set; }

        public int ActualCount { get; set; }
    }
}