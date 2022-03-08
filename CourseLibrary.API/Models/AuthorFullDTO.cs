using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// 03/07/2022 11:08 am - SSN - [20220307-1105] - [001] - M06-06 - Demo - Tightening the contract between client and server with vendor-specific media types

namespace CourseLibrary.API.Models
{
    public class AuthorFullDTO
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTimeOffset DateOfBirth { get; set; }
        public string MainCategory { get; set; }
    }
}
