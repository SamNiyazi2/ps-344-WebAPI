using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseLibrary.API.ResourceParameters
{
    public class AuthorsResourceParameters
    {
        const int maxPageSize = 20;

        public string MainCategory { get; set; }
        public string SearchQuery { get; set; }


        // 03/04/2022 04:52 pm - SSN - [20220304-1649] - [001] - M02-07 - Demo = Paging through collection resources

        public int PageNumber { get; set; } = 1;

        private int _pageSize = 10;

        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > maxPageSize) ? maxPageSize : value;
        }


        // 03/05/2022 06:50 am - SSN - [20220305-0647] - [001] - M03-03 - Demo - Sorting resource collections
        public string OrderBy { get; set; } = "Name";

    }
}
