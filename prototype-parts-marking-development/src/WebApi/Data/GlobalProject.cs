namespace WebApi.Data
{
    using NpgsqlTypes;

    public class GlobalProject
    {
        public int Id { get; set; }

        public string Description { get; set; }

        public string Customer { get; set; }

        public string ProjectNumber { get; set; }

        public NpgsqlTsVector SearchVector { get; set; }
    }
}