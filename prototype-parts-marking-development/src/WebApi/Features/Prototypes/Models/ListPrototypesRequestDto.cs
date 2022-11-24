namespace WebApi.Features.Prototypes.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using Swashbuckle.AspNetCore.Annotations;

    public class ListPrototypesRequestDto
    {
        private const int MaxPageSize = 20;

        private int page = 1;
        private int pageSize = 10;

        [Range(1, double.PositiveInfinity)]
        [SwaggerParameter("Page to retrieve.", Required = false)]
        public int Page
        {
            get => page;
            set => page = Math.Max(1, value);
        }

        [Range(1, MaxPageSize)]
        [SwaggerParameter("Page size.", Required = false)]
        public int PageSize
        {
            get => pageSize;
            set => pageSize = Math.Clamp(value, 1, MaxPageSize);
        }

        public int? Index { get; set; }

        public string Type { get; set; }

        public string SortBy { get; set; }

        public string SortDirection { get; set; }
    }
}