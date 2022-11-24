namespace WebApi.Common.Paging
{
    using System;
    using System.Collections.Generic;

    public class PagedList<T> : List<T>
    {
        public PagedList(IEnumerable<T> items, int totalCount, int currentPage, int pageSize)
            : base(items)
        {
            TotalCount = totalCount;
            PageSize = pageSize;
            CurrentPage = currentPage;
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        }

        public int CurrentPage { get; }

        public int TotalPages { get; }

        public int PageSize { get; }

        public int TotalCount { get; }

        public bool HasPrevious => CurrentPage > 1;

        public bool HasNext => CurrentPage < TotalPages;

        public Pagination CreatePagination()
        {
            return new Pagination
            {
                Page = CurrentPage,
                PageSize = PageSize,
                TotalPages = TotalPages,
                TotalCount = TotalCount,
            };
        }
    }
}
