﻿namespace WebApi.Common.Paging
{
    public class Pagination
    {
        public int TotalCount { get; set; }

        public int PageSize { get; set; }

        public int Page { get; set; }

        public int TotalPages { get; set; }
    }
}