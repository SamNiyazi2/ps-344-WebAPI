using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// 03/04/2022 09:26 pm - SSN - [20220304-2120] - [003] - M02-10 - Demo - Returning pagination metadata

namespace CourseLibrary.API.Helpers
{
    public enum ResourceUriType
    {
        PreviousPage,
        NextPage,
        
        // 03/06/2022 09:03 pm - SSN - [20220306-2054] - [002] - M05-06 - Demo - Implementing HATEOAS support for collection resource

        Current
    }

}
