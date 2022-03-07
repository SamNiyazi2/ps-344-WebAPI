using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// 03/06/2022 07:41 pm - SSN - [20220306-1937] - [001] - M05-04 - Demo - Implementing HATEOAS support for a single resource

namespace CourseLibrary.API.Models
{
    public class LinkDTO
    {
        public string Href { get; private set; }
        public string Rel { get; }
        public string Method { get; }

        public LinkDTO(string href, string rel, string method)
        {
            Href = href;
            Rel = rel;
            Method = method;
        }
    }
}
