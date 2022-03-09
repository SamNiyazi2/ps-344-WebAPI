using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// 03/08/2022 02:38 pm - SSN - [20220307-2140] - [003] - M06-08 - Demo - Working with vendor-specific media types on input

namespace CourseLibrary.API.Models
{
    public class AuthorForCreationWithDateOfDeathDTO : AuthorForCreationDto
    {
        public DateTimeOffset? DateOfDeath { get; set; }
    }
}
