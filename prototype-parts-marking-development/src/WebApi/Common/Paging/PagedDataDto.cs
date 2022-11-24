namespace WebApi.Common.Paging
{
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Mvc;

    public class PagedDataDto<T>
    {
        public List<T> Items { get; set; }

        public Pagination Pagination { get; set; }
    }
}