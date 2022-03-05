using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// 03/04/2022 07:27 pm - SSN - [20220304-1924] - [001] - M02-09 - Demo - Improving reuse with a PagedList<T> class

namespace CourseLibrary.API.Helpers
{
    public class PagedList<T> : List<T>
    {

        public int CurrentPage { get; private set; }
        public int TotalPages { get; private set; }
        public int PageSize { get; private set; }
        public int TotalCount { get; private set; }

        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;

        public PagedList(List<T> items, int totalCount, int pageNumber, int pageSize)
        {
            TotalCount = totalCount;
            PageSize = pageSize;
            CurrentPage = pageNumber;
            TotalPages = (int)Math.Ceiling(Count / (double)pageSize);
            AddRange(items);
        }

        public static PagedList<T> Create(IQueryable<T> source, int pageNumberm, int pageSize)
        {
            var count = source.Count();
            var items = source.Skip((pageNumberm - 1) * pageSize).Take(pageSize).ToList();
            return new PagedList<T>(items, count, pageNumberm, pageSize);

        }

    }
}
